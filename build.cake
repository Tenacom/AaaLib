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
#load "./build/setup-teardown.cake"
#load "./build/version.cake"
#load "./build/workspace.cake"

#nullable enable

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

Task("LocalCleanAll")
    .Description("Like CleanAll, but only runs on a local machine")
    .WithCriteria<BuildData>(data => !data.IsCI)
    .Does<BuildData>(CleanAll);

Task("Restore")
    .Description("Restores dependencies")
    .IsDependentOn("LocalCleanAll")
    .Does<BuildData>(data => RestoreSolution(data));

Task("Build")
    .Description("Build all projects")
    .IsDependentOn("Restore")
    .Does<BuildData>(data => BuildSolution(data, false));

Task("Test")
    .Description("Build all projects and run tests")
    .IsDependentOn("Build")
    .Does<BuildData>(data => TestSolution(data, false, false));

Task("Pack")
    .Description("Build all projects, run tests, and prepare build artifacts")
    .IsDependentOn("Test")
    .Does<BuildData>(data => PackSolution(data, false, false));

Task("Release")
    .Description("Publish a new public release (CI only)")
    .Does<BuildData>(async data => {

        // Preliminary checks
        Ensure(data.IsCI, "The Release target cannot run on a local system.");
        Ensure(data.IsPublicRelease, "Cannot create a release from the current branch.");

        // Create the release as a draft first, so if the token has no permissions we can bail out early
        var releaseId = await CreateDraftReleaseAsync(data);
        var dupeTagChecked = false;
        try
        {
            var committed = false;

            // Advance version if requested.
            var versionAdvance = GetOption<VersionAdvance>("versionAdvance", VersionAdvance.None);
            if (versionAdvance != VersionAdvance.None)
            {
                Information($"Version advance requested: {versionAdvance}.");
                var versionFile = VersionFile.Load();
                var previousVersionSpec = versionFile.VersionSpec;
                if (versionFile.AdvanceVersion(versionAdvance))
                {
                    Information($"Version advanced from {previousVersionSpec} to {versionFile.VersionSpec}.");
                    versionFile.Save();
                    _ = Exec("git", $"add \"{versionFile.Path.FullPath}\"");
                    _ = Exec("git", $"commit -m \"Change version from {previousVersionSpec} to {versionFile.VersionSpec}\"");
                    committed = true;
                }
                else
                {
                    Information("Version not changed.");
                }
            }
            else
            {
                Information("No version advance requested.");
            }

            // Update changelog only on non-prerelease
            if (!data.IsPrerelease)
            {
                if (!GetOption<bool>("checkChangelog", true))
                {
                    Ensure(
                        ChangelogHasUnreleasedChanges(data.ChangelogPath),
                        $"The \"Unreleased changes\" section of the changelog is empty or only contains sub-section headings.");
                }

                // Update the changelog and commit the change before building.
                // This ensures that the Git height is up to date when computing a version for the build artifacts.
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
                Information($"New version after commit is {version}");
                data = data with { Version = version };
            }

            // Ensure that the release tag doesn't already exist.
            // This assumes that full repo history has been checked out;
            // however, that is already a prerequisite for using Nerdbank.GitVersioning.
            Ensure(!GitTagExists(data.Version), $"Tag {data.Version} already exists in repository.");
            dupeTagChecked = true;

            RestoreSolution(data);
            BuildSolution(data, false);
            TestSolution(data, false, false);
            PackSolution(data, false, false);

            if (!data.IsPrerelease)
            {
                // Change the new section's title in the changelog to reflect the actual version.
                UpdateChangelogNewSectionTitle(data);

                // Amend previous commit so Git height doesn't change.
                Information("Amending changelog update commit...");
                _ = Exec("git", $"add \"{data.ChangelogPath.FullPath}\"");
                _ = Exec("git", $"commit --amend -m \"Update changelog\"");
            }

            // Wrap things up: push the commit and deploy artifacts.
            if (committed)
            {
                Information($"Pushing changes to {data.Remote}...");
                _ = Exec("git", $"push {data.Remote} {data.Ref}:{data.Ref}");
            }

            // Publish NuGet packages
            NuGetPushAll(data);

            // If this is not a prerelease and we are releasing from the main branch,
            // dispatch a separate workflow to publish documentation.
            if (!data.IsPrerelease && data.Ref == "main")
            {
                await DispatchWorkflow(data, "deploy-pages.yml", "main");
            }

            // Last but not least, publish the release.
            await PublishReleaseAsync(data, releaseId);
        }
        catch (Exception e)
        {
            Error(e is CakeException ? e.Message : $"{e.GetType().Name}: {e.Message}");
            await DeleteReleaseAsync(data, releaseId, dupeTagChecked ? data.Version : null);
            throw;
        }
    });

// =============================================================================================
// EXECUTION
// =============================================================================================

RunTarget(Argument("target", "Default"));
