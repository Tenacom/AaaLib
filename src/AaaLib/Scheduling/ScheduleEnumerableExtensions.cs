// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using NodaTime;

namespace AaaLib.Scheduling;

/// <summary>
/// Provides extension methods for enumerations of <see cref="ISchedule"/> interfaces.
/// </summary>
public static class ScheduleEnumerableExtensions
{
    /// <summary>
    /// Gets the intersected state of all the schedules in an enumeration at a given moment in time, which is <see langword="true"/> only if
    /// all output states are <see langword="true"/>.
    /// </summary>
    /// <param name="source">The enumerable on which this method is called.</param>
    /// <param name="dateTime">The date and time for which the enumerated schedules are being queried.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// all schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="source"/> contains one or more <see langword="null"/> references.</exception>
    /// <remarks>
    /// <para>If the <paramref name="source"/> enumerable is empty, this method returns <see langword="true"/>.</para>
    /// </remarks>
    public static bool GetIntersectedStateAt(this IEnumerable<ISchedule> source, LocalDateTime dateTime)
    {
        Guard.IsNotNull(source);
        return source.All(schedule =>
        {
            Guard.IsFalse(schedule is null, nameof(source), "Argument contains one or more null values.");
            return schedule.GetStateAt(dateTime);
        });
    }

    /// <summary>
    /// Gets the combined state of all the schedules in an enumeration at a given moment in time, which is <see langword="true"/> if
    /// at least one of the output states is <see langword="true"/>.
    /// </summary>
    /// <param name="source">The enumerable on which this method is called.</param>
    /// <param name="dateTime">The date and time for which the enumerated schedules are being queried.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// at least one schedule returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="source"/> contains one or more <see langword="null"/> references.</exception>
    /// <remarks>
    /// <para>If the <paramref name="source"/> enumerable is empty, this method returns <see langword="false"/>.</para>
    /// </remarks>
    public static bool GetCombinedStateAt(this IEnumerable<ISchedule> source, LocalDateTime dateTime)
    {
        Guard.IsNotNull(source);
        return source.Any(schedule =>
        {
            Guard.IsFalse(schedule is null, nameof(source), "Argument contains one or more null values.");
            return schedule.GetStateAt(dateTime);
        });
    }
}
