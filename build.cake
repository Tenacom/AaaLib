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
#load "./build/public-api.cake"
#load "./build/setup-teardown.cake"
#load "./build/version.cake"
#load "./build/workspace.cake"

#nullable enable

// =============================================================================================
// TASKS
// =============================================================================================

Task("Default")
    .Description("Default task - Do nothing (but log build configuration data)")
    .Does(context => {
        context.Information("The default task does nothing. This is intentional.");
        context.Information("Use `dotnet cake --description` to see the list of available tasks.");
    });

Task("CleanAll")
    .Description("Delete all output directories, VS data, R# caches")
    .Does<BuildData>((context, data) => context.CleanAll(data));

Task("LocalCleanAll")
    .Description("Like CleanAll, but only runs on a local machine")
    .WithCriteria<BuildData>(data => !data.IsCI)
    .Does<BuildData>((context, data) => context.CleanAll(data));

Task("Restore")
    .Description("Restores dependencies")
    .IsDependentOn("LocalCleanAll")
    .Does<BuildData>((context, data) => context.RestoreSolution(data));

Task("Build")
    .Description("Build all projects")
    .IsDependentOn("Restore")
    .Does<BuildData>((context, data) => context.BuildSolution(data, false));

Task("Test")
    .Description("Build all projects and run tests")
    .IsDependentOn("Build")
    .Does<BuildData>((context, data) => context.TestSolution(data, false, false));

Task("Pack")
    .Description("Build all projects, run tests, and prepare build artifacts")
    .IsDependentOn("Test")
    .Does<BuildData>((context, data) => context.PackSolution(data, false, false));

Task("Release")
    .Description("Publish a new public release (CI only)")
    .Does<BuildData>(async (context, data) => {

        // Preliminary checks
        Ensure(data.IsCI, "The Release target cannot run on a local system.");
        Ensure(data.IsPublicRelease, "Cannot create a release from the current branch.");

        // Create the release as a draft first, so if the token has no permissions we can bail out early
        var releaseId = await context.CreateDraftReleaseAsync(data);
        var dupeTagChecked = false;
        try
        {
            var commit = false;

            // Advance version if requested.
            var versionAdvance = context.GetOption<VersionAdvance>("versionAdvance", VersionAdvance.None);
            if (context.GetOption<bool>("checkPublicApi", true))
            {
                var requiredVersionAdvance = context.GetMaxPublicApiRequiredVersionAdvance();
                Ensure(versionAdvance >= requiredVersionAdvance, $"Changes to public API require a minimum version advance of {requiredVersionAdvance}.");
            }

            if (versionAdvance != VersionAdvance.None)
            {
                context.Information($"Version advance requested: {versionAdvance}.");
                var versionFile = VersionFile.Load();
                var previousVersionSpec = versionFile.VersionSpec;
                if (versionFile.AdvanceVersion(versionAdvance))
                {
                    context.Information($"Version advanced from {previousVersionSpec} to {versionFile.VersionSpec}.");
                    versionFile.Save();
                    _ = context.Exec("git", $"add \"{versionFile.Path.FullPath}\"");
                    commit = true;
                }
                else
                {
                    context.Information("Version not changed.");
                }
            }
            else
            {
                context.Information("No version advance requested.");
            }

            // Update public API files only on non-prerelease
            if (!data.IsPrerelease)
            {
                var modified = context.TransferAllPublicApiToShipped().ToArray();
                if (modified.Length > 0)
                {
                    context.Information($"{modified.Length} public API files were modified.");
                    foreach (var path in modified)
                    {
                        _ = context.Exec("git", $"add \"{path.FullPath}\"");
                    }

                    commit = true;
                }
                else
                {
                    context.Information("No public API files were updated.");
                }
            }

            // Update changelog only on non-prerelease
            if (!data.IsPrerelease)
            {
                if (context.GetOption<bool>("checkChangelog", true))
                {
                    Ensure(
                        context.ChangelogHasUnreleasedChanges(data.ChangelogPath),
                        $"The \"Unreleased changes\" section of the changelog is empty or only contains sub-section headings.");
                }

                // Update the changelog and commit the change before building.
                // This ensures that the Git height is up to date when computing a version for the build artifacts.
                context.PrepareChangelogForRelease(data);
                _ = context.Exec("git", $"add \"{data.ChangelogPath.FullPath}\"");
                commit = true;
            }

            if (commit)
            {
                context.Information("Committing changed files...");
                _ = context.Exec("git", $"commit -m \"Prepare release\"");
                data.Update(context);
            }

            // Ensure that the release tag doesn't already exist.
            // This assumes that full repo history has been checked out;
            // however, that is already a prerequisite for using Nerdbank.GitVersioning.
            Ensure(!context.GitTagExists(data.Version), $"Tag {data.Version} already exists in repository.");
            dupeTagChecked = true;

            context.RestoreSolution(data);
            context.BuildSolution(data, false);
            context.TestSolution(data, false, false);
            context.PackSolution(data, false, false);

            if (!data.IsPrerelease)
            {
                // Change the new section's title in the changelog to reflect the actual version.
                context.UpdateChangelogNewSectionTitle(data);
                _ = context.Exec("git", $"add \"{data.ChangelogPath.FullPath}\"");
            }

            if (commit)
            {
                context.Information("Amending commit...");
                _ = context.Exec("git", $"commit --amend -m \"Prepare release {data.Version}\"");
                context.Information($"Pushing changes to {data.Remote}...");
                _ = context.Exec("git", $"push {data.Remote} {data.Ref}:{data.Ref}");
            }

            // Publish NuGet packages
            context.NuGetPushAll(data);

            // If this is not a prerelease and we are releasing from the main branch,
            // dispatch a separate workflow to publish documentation.
            if (!data.IsPrerelease && data.Ref == "main")
            {
                await context.DispatchWorkflow(data, "deploy-pages.yml", "main");
            }

            // Last but not least, publish the release.
            await context.PublishReleaseAsync(data, releaseId);
        }
        catch (Exception e)
        {
            context.Error(e is CakeException ? e.Message : $"{e.GetType().Name}: {e.Message}");
            await context.DeleteReleaseAsync(data, releaseId, dupeTagChecked ? data.Version : null);
            throw;
        }
    });

// =============================================================================================
// EXECUTION
// =============================================================================================

RunTarget(Argument("target", "Default"));
