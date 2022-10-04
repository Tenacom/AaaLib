// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#load "./build/BuildData.cake"
#load "./build/changelog.cake"
#load "./build/dotnet.cake"
#load "./build/environment.cake"
#load "./build/fail.cake"
#load "./build/filesystem.cake"
#load "./build/git.cake"
#load "./build/github.cake"
#load "./build/json.cake"
#load "./build/nbgv.cake"
#load "./build/options.cake"
#load "./build/process.cake"
#load "./build/workspace.cake"

#nullable enable

// =============================================================================================
// SETUP / TEARDOWN
// =============================================================================================

Setup<BuildData>(context =>
{
    var data = CreateBuildData();
    Information($"Repository        : {data.RepositoryOwner}/{data.RepositoryName}");
    Information($"Git remote name   : {data.Remote}");
    Information($"Git reference     : {data.Ref}");
    Information($"Branch            : {data.Branch}");
    Information($"Build environment : {(data.IsCI ? "cloud" : "local")}");
    Information($"Solution          : {data.SolutionPath.GetFilename()}");
    Information($"Version           : {data.Version}");
    Information($"Public release    : {(data.IsPublicRelease ? "yes" : "no")}");
    Information($"Prerelease        : {(data.IsPrerelease ? "yes" : "no")}");

    if (data.IsCI && !data.IsGitHubAction)
    {
        throw new CakeException(255, "This script can only run locally or in a GitHub Actions workflow.");
    }

    return data;
});

Teardown<BuildData>((context, data) => {

    // For some reason, DotNetBuildServerShutdown hangs in a GitHub Actions runner;
    // it is still useful on a local machine though
    if (!data.IsCI)
    {
        context.DotNetBuildServerShutdown(new DotNetBuildServerShutdownSettings
        {
            Razor = true,
            VBCSCompiler = true,
        });
    }
});

// =============================================================================================
// TASKS
// =============================================================================================

Task("Default")
    .Description("Default task - Do nothing (but log build configuration data)")
    .Does<BuildData>(_ => {
        Information("The default task does nothing. This is intentional.");
        Information("Use `dotnet cake --description` to see the list of available tasks.");
    });

Task("CleanAll")
    .Description("Delete all output directories, VS data, R# caches")
    .Does<BuildData>(CleanAll);

Task("Verify")
    .Description("Build all projects, run tests, and build artifacts")
    .Does<BuildData>(data => {
        if (!data.IsCI)
        {
            CleanAll(data);
        }

        RestoreSolution(data);
        BuildSolution(data, false);
        TestSolution(data, false, false);
        PackSolution(data, false, false);
    });

Task("Release")
    .Description("Publish a new public release")
    .Does<BuildData>(async data => {

        // Preliminary checks
        Ensure(data.IsCI, "The Release target cannot run on a local system.");
        Ensure(data.IsPublicRelease, "Cannot create a release from the current branch.");

        // Create the release as a draft first, so if the token has no permissions we can bail out early
        var releaseId = await CreateDraftReleaseAsync(data);
        try
        {
            // Update changelog only on non-prerelease
            var committed = false;
            if (!data.IsPrerelease)
            {
                if (!GetOption<bool>("skipChangelogCheck", false))
                {
                    Ensure(
                        ChangelogHasUnreleasedChanges(data.ChangelogPath),
                        $"The \"Unreleased changes\" section of the changelog is empty or only contains sub-section headings.");
                }

                // Update the changelog and commit the change before building.
                // This ensures that the Git height is up to date in the built artifacts' version.
                PrepareChangelogForRelease(data);

                Information("Committing updated changelog...");
                _ = Exec("git", $"add \"{data.ChangelogPath.FullPath}\"");
                _ = Exec("git", $"commit -m \"Update changelog\"");
                committed = true;
            }

            if (committed)
            {
                // The commit changed the Git height: update build configuration data to reflect the change.
                (var version, _, _, _) = GetVersionInformation();
                Information("New version after commit is {version}");
                data = data with { Version = version };
            }

            // Ensure that the release tag doesn't already exist.
            // This assumes that full repo history has been checked out;
            // however, that is already a prerequisite for using Nerdbank.GitVersioning.
            Ensure(!GitTagExists(data.Version), "Tag {data.Version} already exists in repository.");

            RestoreSolution(data);
            BuildSolution(data, false);
            TestSolution(data, false, false);
            PackSolution(data, false, false);

            // Wrap things up: push the commit and deploy artifacts.
            if (committed)
            {
                Information("Pushing changes to {data.Remote}...");
                _ = Exec("git", $"push {data.Remote} {data.Ref}:{data.Ref}");
            }

            NuGetPushAll(data);
        }
        catch (Exception e)
        {
            Error($"{e.GetType().Name}: {e.Message}");
            await DeleteReleaseAsync(data, releaseId);
            throw;
        }

        await PublishReleaseAsync(data, releaseId);
    });

// =============================================================================================
// EXECUTION
// =============================================================================================

RunTarget(Argument("target", "Default"));
