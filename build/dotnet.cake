// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// .NET SDK helpers
// ---------------------------------------------------------------------------------------------

using System.IO;
using System.Linq;

using SysDirectory = System.IO.Directory;
using SysPath = System.IO.Path;

/*
 * Summary : Restore all NuGet packages for the solution.
 * Params  : data - Build configuration data.
 */
void RestoreSolution(BuildData data)
{
    Information("Restoring NuGet packages for solution...");
    DotNetRestore(data.SolutionPath.FullPath, new() {
        DisableParallel = true,
        Interactive = false,
        MSBuildSettings = data.MSBuildSettings,
    });
}

/*
 * Summary : Build all projects in teh solution.
 * Params  : data    - Build configuration data.
 *           restore - true to restore NuGet packages before building, false otherwise.
 */
void BuildSolution(BuildData data, bool restore)
{
    Information($"Building solution (restore = {restore})...");
    DotNetBuild(data.SolutionPath.FullPath, new() {
        Configuration = data.Configuration,
        MSBuildSettings = data.MSBuildSettings,
        NoLogo = true,
        NoRestore = !restore,
    });
}

/*
 * Summary : Run all unit tests for the solution.
 * Params  : data    - Build configuration data.
 *           restore - true to restore NuGet packages before testing, false otherwise.
 *           build   - true to build the solution before testing, false otherwise.
 */
void TestSolution(BuildData data, bool restore, bool build)
{
    Information($"Running tests (restore = {restore}, build = {build})...");
    DotNetTest(data.SolutionPath.FullPath, new() {
        Configuration = data.Configuration,
        NoBuild = !build,
        NoLogo = true,
        NoRestore = !restore,
    });
}

/*
 * Summary : Run the Pack target on the solution. This usually produces NuGet packages,
 *           but Buildvana SDK may hijack the target to produce, for example, setup executables.
 * Params  : data    - Build configuration data.
 *           restore - true to restore NuGet packages before packing, false otherwise.
 *           build   - true to build the solution before packing, false otherwise.
 */
void PackSolution(BuildData data, bool restore, bool build)
{
    Information($"Packing solution (restore = {restore}, build = {build})...");
    DotNetPack(data.SolutionPath.FullPath, new() {
        Configuration = data.Configuration,
        MSBuildSettings = data.MSBuildSettings,
        NoBuild = !build,
        NoLogo = true,
        NoRestore = !restore,
    });
}

/*
 * Summary : Push all produced NuGet packages to the appropriate NuGet server.
 * Params  : data - Build configuration data.
 * Remarks : - This method uses the following environment variables:
 *             * PRERELEASE_NUGET_SOURCE - NuGet source URL where to push prerelease packages
 *             * RELEASE_NUGET_SOURCE    - NuGet source URL where to push non-prerelease packages
 *             * PRERELEASE_NUGET_KEY    - API key for PRERELEASE_NUGET_SOURCE
 *             * RELEASE_NUGET_KEY       - API key for RELEASE_NUGET_SOURCE
 *           - If there are no .nupkg files in the designated artifacts directory, this method does nothing.
 */
void NuGetPushAll(BuildData data)
{
    const string nupkgMask = "*.nupkg";
    if (!SysDirectory.EnumerateFiles(data.ArtifactsPath.FullPath, nupkgMask).Any())
    {
        Verbose("No .nupkg files to push.");
        return;
    }

    var nugetSource = GetOptionOrFail<string>(data.IsPrerelease ? "prereleaseNugetSource" : "releaseNugetSource");
    var nugetApiKey = GetOptionOrFail<string>(data.IsPrerelease ? "prereleaseNugetKey" : "releaseNugetKey");
    var nugetPushSettings = new DotNetNuGetPushSettings {
        ForceEnglishOutput = true,
        Source = nugetSource,
        ApiKey = nugetApiKey,
        SkipDuplicate = true,
    };

    var packages = SysPath.Combine(data.ArtifactsPath.FullPath, nupkgMask);
    foreach (var path in GetFiles(packages))
    {
        Information($"Pushing {path} to {nugetSource}...");
        DotNetNuGetPush(path, nugetPushSettings);
    }
}
