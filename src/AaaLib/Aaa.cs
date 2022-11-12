﻿// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace AaaLib;

/// <summary>
/// Provides proof that this project has XML docs.
/// </summary>
public static class Aaa
{
    private const string Quarantadue = nameof(Quarantadue);

    /// <summary>
    /// Gets the answer to the ultimate question of life, the universe, and everything.
    /// </summary>
    /// <returns>The answer.</returns>
    /// <remarks>
    /// <para>I just wish someone would remind me what the question was.</para>
    /// </remarks>
    public static int GetTheAnswer() => 42;

    /// <summary>
    /// Gets the answer to the ultimate question of life, the universe, and everything,
    /// as a read-only span of characters, in Italian.
    /// </summary>
    /// <returns>La risposta ("the answer", in Italian).</returns>
    public static ReadOnlySpan<char> GetTheAnswerInItalian() => Quarantadue.AsSpan();
}