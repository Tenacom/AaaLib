// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using NodaTime;

namespace AaaLib.Scheduling;

/// <summary>
/// Represents a scheduled activation during a given time range of every day.
/// </summary>
/// <param name="StartTime">The start time of the schedule, i.e. the time at which the output state becomes <see langword="true"/>.</param>
/// <param name="EndTime">The end time of the schedule, i.e. the time at which the output state becomes <see langword="false"/>.</param>
/// <remarks>
/// <para>If <paramref name="EndTime"/> is set to a time earlier than <paramref name="StartTime"/>, the output state remains <see langword="true"/>
/// across midnights. For example, if <paramref name="StartTime"/> is set to 20:00 (8:00p.m.) and <paramref name="EndTime"/> is set to 10:00 (10:00a.m.),
/// the output state will be <see langword="true"/> from midnight (0:00) to 10:00, then <see langword="false"/> from 10:00 to 20:00, then
/// <see langword="true"/> again from 20:00 to midnight.</para>
/// <para>If <paramref name="StartTime"/> and <paramref name="EndTime"/> represent the same time, the output state will always be <see langword="false"/>.</para>
/// </remarks>
public sealed record DailySchedule(
    LocalTime StartTime,
    LocalTime EndTime) : ISchedule
{
    /// <inheritdoc/>
    /// <returns><see langword="true"/> if the time of day in <paramref name="dateTime"/> is between <see cref="StartTime"/> and <see cref="EndTime"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public bool GetStateAt(LocalDateTime dateTime)
    {
        var time = dateTime.TimeOfDay;
        return StartTime <= EndTime
                ? (StartTime <= time && EndTime > time)
                : (StartTime <= time || EndTime > time);
    }
}
