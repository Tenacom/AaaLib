// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// BuildData: a record to hold build configuration data
// ---------------------------------------------------------------------------------------------

/*
 * Summary : Holds configuration data for the build.
 */
sealed record BuildData(

    /*
     * Summary : Gets the repository owner (e.g. "Tenacom" for repository Tenacom/SomeLibrary.)
     */
    string RepositoryOwner,

    /*
     * Summary : Gets the repository owner (e.g. "SomeLibrary" for repository Tenacom/SomeLibrary.)
     */
    string RepositoryName,

    /*
     * Summary : Gets the name of the Git remote that points to the main repository
     *           (usually "origin" in cloud builds, "upstream" when working locally on a fork.)
     */
    string Remote,

    /*
     * Summary : Gets Git's HEAD reference or SHA.
     */
    string Ref,

    /*
     * Summary : Gets Git's HEAD branch name, or the empty string if not on a branch.
     */
    string Branch,

    /*
     * Summary : Gets the path of the directory where build artifacts are stored.
     */
    DirectoryPath ArtifactsPath,

    /*
     * Summary : Gets the path of the CHANGELOG.md file.
     */
    FilePath ChangelogPath,

    /*
     * Summary : Gets the path of the solution file.
     */
    FilePath SolutionPath,

    /*
     * Summary : Gets the parsed solution.
     */
    SolutionParserResult Solution,

    /*
     * Summary : Gets the configuration to build.
     */
    string Configuration,

    /*
     * Summary : Gets the version to build, as computed by Nerdbank.GitVersioning.
     */
    string Version,

    /*
     * Summary : Gets a value that indicates whether a public release can be built.
     * Value   : True if Git's HEAD is on a public release branch, as indicated in version.json;
     *           otherwise, false.
     */
    bool IsPublicRelease,

    /*
     * Summary : Gets a value that indicates whether the version to build is a prerelease.
     */
    bool IsPrerelease,

    /*
     * Summary : Gets a value that indicates whether Cake is running in a GitHub Actions workflow.
     */
    bool IsGitHubAction,

    /*
     * Summary : Gets a value that indicates whether Cake is running on a cloud build server.
     */
    bool IsCI,

    /*
     * Summary : Gets the MSBuild settings to use for DotNet aliases.
     */
    DotNetMSBuildSettings MSBuildSettings);

/*
 * Summary : Initializes a new instance of the BuildData class.
 * Params  : context - The Cake context.
 */
BuildData CreateBuildData()
{
    Ensure(TryGetRepositoryInfo(out var repository), 255, "Cannot determine repository owner and name.");
    var changelogPath = new FilePath("CHANGELOG.md");
    var solutionPath = GetFiles("*.sln").FirstOrDefault() ?? Fail<FilePath>(255, "Cannot find a solution file.");
    var solution = ParseSolution(solutionPath);
    var configuration = Argument("configuration", "Release");
    var artifactsPath = new DirectoryPath("artifacts").Combine(configuration);
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

    var (version, @ref, isPublicRelease, isPrerelease) = GetVersionInformation();
    var branch = GetCurrentGitBranch();
    var msBuildSettings = new DotNetMSBuildSettings {
        MaxCpuCount = 1,
        ContinuousIntegrationBuild = isCI,
        NoLogo = true,
    };

    return new(
        RepositoryOwner: repository.Owner,
        RepositoryName: repository.Name,
        Remote: repository.Remote,
        Ref: @ref,
        Branch: branch,
        ArtifactsPath: artifactsPath,
        ChangelogPath: changelogPath,
        SolutionPath: solutionPath,
        Solution: solution,
        Configuration: configuration,
        Version: version,
        IsPublicRelease: isPublicRelease,
        IsPrerelease: isPrerelease,
        IsGitHubAction: isGitHubAction,
        IsCI: isCI,
        MSBuildSettings: msBuildSettings);
}
