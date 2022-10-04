// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// Process helpers
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;

/*
 * Summary : Executes an external command, capturing standard output and failing if the exit code is not zero.
 * Params  : command   - The name of the command to execute.
 *           arguments - The arguments to pass to the command.
 * Returns : The captured output of the command.
 */
IEnumerable<string> Exec(string command, ProcessArgumentBuilder arguments)
{
    var exitCode = Exec(command, arguments, out var output);
    Ensure(exitCode == 0, $"'{command} {arguments.RenderSafe()}' exited with code {exitCode}.");
    return output;
}

/*
 * Summary : Executes an external command, capturing standard output and failing if the exit code is not zero.
 * Params  : command    - The name of the command to execute.
 *           arguments  - The arguments to pass to the command.
 *           out output - The captured output of the command.
 * Returns : The exit code of the command.
 */
int Exec(string command, ProcessArgumentBuilder arguments, out IEnumerable<string> output)
    => StartProcess(
        command,
        new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true },
        out output);
