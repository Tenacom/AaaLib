// Copyright (C) Tenacom and Contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#tool dotnet:?package=nbgv&version=3.5.109

#nullable enable

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

// ARGUMENT: --target <target>
//           Execute task "<target>".
//           Default value: "Default"
var target = Argument("target", "Default");

//==============================================================================================
// CONSTANTS
//==============================================================================================

const string NuGetReleaseSource = "https://api.nuget.org/v3/index.json";
const string NuGetPrereleaseSource = "https://www.myget.org/F/tenacom-preview/api/v3/index.json";

//==============================================================================================
// SETUP / TEARDOWN
//==============================================================================================

public record BuildData(
    FilePath SolutionPath,
    SolutionParserResult Solution,
    string Configuration,
    string Version,
    bool IsPublicRelease,
    bool IsPrerelease,
    bool IsGitHubAction,
    bool IsCI,
    DotNetMSBuildSettings MSBuildSettings
);

Setup<BuildData>(context =>
{
    // ARGUMENT: --configuration <configuration>
    //           Use configuration "<configuration>" to build the solution.
    //           Default value: "Release"
    var solutionPath = GetFiles("*.sln").FirstOrDefault() ?? throw new CakeException(255, "Cannot find a solution file.");
    var solution = ParseSolution(solutionPath);
    var configuration = Argument("configuration", "Release");
    var isGitHubAction = EnvironmentVariable<bool>("GITHUB_ACTIONS", false);
    var isCI = isGitHubAction
            || EnvironmentVariable<bool>("CI", false)
            || EnvironmentVariable<bool>("CONTINUOUS_INTEGRATION", false)
            || EnvironmentVariable<bool>("TF_BUILD", false)
            || EnvironmentVariable<bool>("GITLAB_CI", false)
            || EnvironmentVariable<bool>("TRAVIS", false)
            || EnvironmentVariable<bool>("APPVEYOR", false)
            || EnvironmentVariable<bool>("CIRCLECI", false)
            || HasEnvironmentVariable("TEAMCITY_VERSION")
            || HasEnvironmentVariable("JENKINS_URL");

    var (version, @ref, isPublicRelease, isPrerelease) = GetVersionInformation(context);
    var msbuildSettings = new DotNetMSBuildSettings {
        MaxCpuCount = 1,
        ContinuousIntegrationBuild = isCI,
        NoLogo = true,
    };

    Information($"{(isCI ? "CI" : "Local")} build of {solutionPath.GetFilenameWithoutExtension()} v{version} ({(isPublicRelease ? "public" : "private")} {(isPrerelease ? "pre" : null)}release from {@ref})");
    return new(
        SolutionPath: solutionPath,
        Solution: solution,
        Configuration: configuration,
        Version: version,
        IsPublicRelease: isPublicRelease,
        IsPrerelease: isPrerelease,
        IsGitHubAction: isGitHubAction,
        IsCI: isCI,
        MSBuildSettings: msbuildSettings
    );
});

Teardown(context => {
    context.DotNetBuildServerShutdown(new DotNetBuildServerShutdownSettings
    {
        Razor = true,
        VBCSCompiler = true,
    });
});

//==============================================================================================
// TASKS
//==============================================================================================

Task("LocalClean")
    .Description("Delete all output, intermediate output, caches, artifacts, and logs - Only runs on local builds")
    .WithCriteria<BuildData>((_, data) => !data.IsCI)
    .Does<BuildData>(data =>
    {
        var deleteDirectorySettings = new DeleteDirectorySettings() { Force = false, Recursive = true };
        DeleteDir(".vs");
        DeleteDir("_ReSharper.Caches");
        DeleteDir("artifacts");
        DeleteDir("logs");
        foreach (var project in data.Solution.Projects)
        {
            var projectDirectory = project.Path.GetDirectory();
            DeleteDir(projectDirectory.Combine("bin"));
            DeleteDir(projectDirectory.Combine("obj"));
        }

        void DeleteDir(DirectoryPath directory)
        {
            if (!DirectoryExists(directory))
            {
                Verbose($"Skipping non-existent directory: {directory}");
                return;
            }

            Information($"Deleting directory: {directory}");
            DeleteDirectory(directory, deleteDirectorySettings);
        }
    });

Task("Init")
    .Description("Perform initializations - Equivalent to LocalClean")
    .IsDependentOn("LocalClean");

Task("Restore")
    .Description("Restore dependencies")
    .Does<BuildData>(data =>
    {
        DotNetRestore(data.SolutionPath.FullPath, new() {
            DisableParallel = true,
            Interactive = false,
            MSBuildSettings = data.MSBuildSettings,
        });
    });

Task("Build")
    .Description("Build all projects in the specified configuration")
    .IsDependentOn("Restore")
    .Does<BuildData>(data =>
    {
        DotNetBuild(data.SolutionPath.FullPath, new DotNetBuildSettings
        {
            Configuration = data.Configuration,
            MSBuildSettings = data.MSBuildSettings,
            NoLogo = true,
            NoRestore = true,
        });
    });

Task("Test")
    .Description("Run unit tests and collects code coverage information")
    .IsDependentOn("Build")
    .Does<BuildData>(data =>
    {
        DotNetTest(data.SolutionPath.FullPath, new DotNetTestSettings
        {
            Configuration = data.Configuration,
            NoBuild = true,
            NoLogo = true,
        });
    });

Task("Pack")
    .Description("Prepare deployable artifacts")
    .IsDependentOn("Build")
    .Does<BuildData>(data =>
    {
        DotNetPack(data.SolutionPath.FullPath, new DotNetPackSettings
        {
            Configuration = data.Configuration,
            MSBuildSettings = data.MSBuildSettings,
            NoBuild = true,
            NoLogo = true,
        });
    });

Task("DeployNuGet")
    .Description("Push NuGet packages")
    .WithCriteria<BuildData>((_, data) => data.IsCI && data.IsPublicRelease)
    .IsDependentOn("Pack")
    .Does<BuildData>(data =>
    {
        var nuGetApiKeyVariable = data.IsPrerelease ? "PRERELEASE_DEPLOYMENT_KEY" : "RELEASE_DEPLOYMENT_KEY";
        var nuGetApiKey = EnvironmentVariable<string>(nuGetApiKeyVariable, string.Empty);
        if (string.IsNullOrEmpty(nuGetApiKey))
        {
            throw new CakeException(255, $"Environment variable {nuGetApiKeyVariable} is missing or has an empty value.");
        }

        var nuGetPushSettings = new DotNetNuGetPushSettings {
            Source = data.IsPrerelease ? NuGetPrereleaseSource : NuGetReleaseSource,
            ApiKey = nuGetApiKey,
            SkipDuplicate = true,
        };

        var packages = SysPath.Combine("artifacts", data.Configuration, "*.nupkg");

        DotNetNuGetPush(packages, nuGetPushSettings);
    });

Task("Deploy")
    .Description("Deploy artifacts - Only runs on CI builds")
    .WithCriteria<BuildData>((_, data) => data.IsCI && data.IsPublicRelease)
    .IsDependentOn("DeployNuGet");

Task("Default")
    .Description("Default task - Equivalent to Init + Test + Pack")
    .IsDependentOn("Init")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    .IsDependentOn("Deploy");

//==============================================================================================
// EXECUTION
//==============================================================================================

RunTarget(target);

//==============================================================================================
// UTILITY METHODS
//==============================================================================================

static (string Version, string Ref, bool IsPublicRelease, bool IsPrerelease) GetVersionInformation(ICakeContext context)
{
    // Use nbgv to get version information.
    var nbgv = context.Tools.Resolve("nbgv.dll");
    var nbgvOutput = new StringBuilder();
    context.DotNetExecute(
        nbgv,
        "get-version --format json",
        new DotNetExecuteSettings {
            SetupProcessSettings = s => s
                .SetRedirectStandardOutput(true)
                .SetRedirectedStandardOutputHandler(x => {
                    nbgvOutput.AppendLine(x);
                    return x;
                }),
        });

    var json = ParseJsonObject(nbgvOutput.ToString(), "The output of nbgv");
    return (
        Version: GetJsonPropertyValue<string>(json, "NuGetPackageVersion", "the output of nbgv"),
        Ref: GetJsonPropertyValue<string>(json, "BuildingRef", "the output of nbgv"),
        IsPublicRelease: GetJsonPropertyValue<bool>(json, "PublicRelease", "the output of nbgv"),
        IsPrerelease: !string.IsNullOrEmpty(GetJsonPropertyValue<string>(json, "PrereleaseVersion", "the output of nbgv")));
}

static JsonObject ParseJsonObject(string str, string description = "The provided string")
{
    JsonNode? node;
    try
    {
        node = JsonNode.Parse(str);
    }
    catch (JsonException e)
    {
        throw new CakeException(255, $"{description} is not valid JSON.", e);
    }

    return node switch {
        null => throw new CakeException(255, $"{description} was parsed as JSON null."),
        JsonObject obj => obj,
        object other => throw new CakeException(255, $"{description} was parsed as a {other.GetType().Name}, not a {nameof(JsonObject)}."),
    };
}

static T GetJsonPropertyValue<T>(JsonObject json, string propertyName, string objectDescription = "JSON object")
{
    if (!json.TryGetPropertyValue(propertyName, out var property))
    {
        throw new CakeException(255, $"Json property {propertyName} not found in {objectDescription}.");
    }

    switch (property)
    {
        case null:
            throw new CakeException(255, $"Json property {propertyName} in {objectDescription} is null.");
        case JsonValue value:
            if (!value.TryGetValue<T>(out var result))
            {
                throw new CakeException(255, $"Json property {propertyName} in {objectDescription} cannot be converted to a {typeof(T).Name}.");
            }

            return result ?? throw new CakeException(255, $"Json property {propertyName} in {objectDescription} has a null value.");
        default:
            throw new CakeException(255, $"Json property {propertyName} in {objectDescription} is a {property.GetType().Name}, not a {nameof(JsonValue)}.");
    }
}
