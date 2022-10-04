// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// File system helpers
// ---------------------------------------------------------------------------------------------

/*
 * Summary : Delete a directory, including its contents, if it exists.
 * Params  : directory - The directory to delete.
 */
void DeleteDirectoryIfExists(DirectoryPath directory)
{
    if (!DirectoryExists(directory))
    {
        Verbose($"Skipping non-existent directory: {directory}");
        return;
    }

    Information($"Deleting directory: {directory}");
    DeleteDirectory(directory, new() { Force = false, Recursive = true });
}
