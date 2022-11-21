// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using CommunityToolkit.Diagnostics;
using NodaTime;

namespace AaaLib.Scheduling;

/// <summary>
/// Provides helpers to manage schedules.
/// </summary>
public static partial class Schedule
{
    /// <summary>
    /// Gets an <see cref="ISchedule"/> interface whose output state is always <see langword="true"/>.
    /// </summary>
    public static ISchedule Always { get; } = new AlwaysSchedule();

    /// <summary>
    /// Gets an <see cref="ISchedule"/> interface whose output state is always <see langword="false"/>.
    /// </summary>
    public static ISchedule Never { get; } = new NeverSchedule();

    /// <summary>
    /// Gets an <see cref="ISchedule"/> interface whose output state is always equal to a given state.
    /// </summary>
    /// <param name="state">The desired state.</param>
    /// <returns>An <see cref="ISchedule"/> interface.</returns>
    public static ISchedule GetConstantSchedule(bool state) => state ? Always : Never;

    /// <summary>
    /// Gets the intersected state of two schedules at a given moment in time, which is <see langword="true"/> only if
    /// both output states are <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="first">The first schedule.</param>
    /// <param name="second">The second schedule.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// both schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="first"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="second"/> is <see langword="null"/></para>
    /// </exception>
    public static bool GetIntersectedStateAt(LocalDateTime dateTime, ISchedule first, ISchedule second)
    {
        Guard.IsNotNull(first);
        Guard.IsNotNull(second);

        return first.GetStateAt(dateTime) && second.GetStateAt(dateTime);
    }

    /// <summary>
    /// Gets the intersected state of three schedules at a given moment in time, which is <see langword="true"/> only if
    /// all three output states are <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="first">The first schedule.</param>
    /// <param name="second">The second schedule.</param>
    /// <param name="third">The third schedule.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// all three schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="first"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="second"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="third"/> is <see langword="null"/></para>
    /// </exception>
    public static bool GetIntersectedStateAt(LocalDateTime dateTime, ISchedule first, ISchedule second, ISchedule third)
    {
        Guard.IsNotNull(first);
        Guard.IsNotNull(second);
        Guard.IsNotNull(third);

        return first.GetStateAt(dateTime) && second.GetStateAt(dateTime) && third.GetStateAt(dateTime);
    }

    /// <summary>
    /// Gets the intersected state of an arbitrary number of schedules at a given moment in time, which is <see langword="true"/> only if
    /// all output states are <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="schedules">An array of <see cref="ISchedule"/> interfaces.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// all schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="schedules"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="schedules"/> contains one or more <see langword="null"/> references.</exception>
    /// <remarks>
    /// <para>If the <paramref name="schedules"/> array is empty, this method returns <see langword="true"/>.</para>
    /// </remarks>
    public static bool GetIntersectedStateAt(LocalDateTime dateTime, params ISchedule[] schedules)
    {
        Guard.IsNotNull(schedules);
        foreach (var schedule in schedules)
        {
            Guard.IsFalse(schedule is null, nameof(schedules), "Argument contains one or more null values.");
            if (!schedule.GetStateAt(dateTime))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the combined state of two schedules at a given moment in time, which is <see langword="true"/> if
    /// at least one of the output states is <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="first">The first schedule.</param>
    /// <param name="second">The second schedule.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// at least one of the schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="first"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="second"/> is <see langword="null"/></para>
    /// </exception>
    public static bool GetCombinedStateAt(LocalDateTime dateTime, ISchedule first, ISchedule second)
    {
        Guard.IsNotNull(first);
        Guard.IsNotNull(second);

        return first.GetStateAt(dateTime) || second.GetStateAt(dateTime);
    }

    /// <summary>
    /// Gets the combined state of three schedules at a given moment in time, which is <see langword="true"/> if
    /// at least one of the output states is <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="first">The first schedule.</param>
    /// <param name="second">The second schedule.</param>
    /// <param name="third">The third schedule.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// at least one of the schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="first"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="second"/> is <see langword="null"/></para>
    /// <para>- or -</para>
    /// <para><paramref name="third"/> is <see langword="null"/></para>
    /// </exception>
    public static bool GetCombinedStateAt(LocalDateTime dateTime, ISchedule first, ISchedule second, ISchedule third)
    {
        Guard.IsNotNull(first);
        Guard.IsNotNull(second);
        Guard.IsNotNull(third);

        return first.GetStateAt(dateTime) || second.GetStateAt(dateTime) || third.GetStateAt(dateTime);
    }

    /// <summary>
    /// Gets the combined state of an arbitrary number of schedules at a given moment in time, which is <see langword="true"/> if
    /// at least one of the output states is <see langword="true"/>.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedules are being queried.</param>
    /// <param name="schedules">An array of <see cref="ISchedule"/> interfaces.</param>
    /// <returns><see langword="true"/> if the <see cref="ISchedule.GetStateAt">GetStateAt</see> method of
    /// at least one of the schedules returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="schedules"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="schedules"/> contains one or more <see langword="null"/> references.</exception>
    /// <remarks>
    /// <para>If the <paramref name="schedules"/> array is empty, this method returns <see langword="false"/>.</para>
    /// </remarks>
    public static bool GetCombinedStateAt(LocalDateTime dateTime, params ISchedule[] schedules)
    {
        Guard.IsNotNull(schedules);
        foreach (var schedule in schedules)
        {
            Guard.IsFalse(schedule is null, nameof(schedules), "Argument contains one or more null values.");
            if (schedule.GetStateAt(dateTime))
            {
                return true;
            }
        }

        return false;
    }
}
