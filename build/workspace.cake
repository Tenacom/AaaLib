// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// Workspace helpers
// ---------------------------------------------------------------------------------------------

/*
 * Summary : Delete all intermediate and output directories.
 *           On a local machine, also delete Visual Studio and ReSharper caches.
 */
void CleanAll(BuildData data)
{
    DeleteDirectoryIfExists(".vs");
    DeleteDirectoryIfExists("_ReSharper.Caches");
    DeleteDirectoryIfExists("artifacts");
    DeleteDirectoryIfExists("logs");
    foreach (var project in data.Solution.Projects)
    {
        var projectDirectory = project.Path.GetDirectory();
        DeleteDirectoryIfExists(projectDirectory.Combine("bin"));
        DeleteDirectoryIfExists(projectDirectory.Combine("obj"));
    }
}
