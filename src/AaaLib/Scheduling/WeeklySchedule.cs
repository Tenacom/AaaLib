// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using NodaTime;

namespace AaaLib.Scheduling;

/// <summary>
/// Represents a scheduled activation on one or more given days of the week.
/// </summary>
/// <param name="DaysOfWeek">A combination of flags representing the days of the week on which the output state becomes <see langword="true"/>.</param>
public sealed record WeeklySchedule(ScheduledDaysOfWeek DaysOfWeek) : ISchedule
{
    /// <summary>
    /// Determines whether a weekly schedule is enabled on a given day of the week.
    /// </summary>
    /// <param name="day">The day of the week to consider.</param>
    /// <param name="days">The schedule's days.</param>
    /// <returns>
    /// <see langword="true"/> if the schedule is enabled on <paramref name="day"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsScheduledDayOfWeek(IsoDayOfWeek day, ScheduledDaysOfWeek days)
        => day switch {
            IsoDayOfWeek.Monday => (days & ScheduledDaysOfWeek.Monday) != 0,
            IsoDayOfWeek.Tuesday => (days & ScheduledDaysOfWeek.Tuesday) != 0,
            IsoDayOfWeek.Wednesday => (days & ScheduledDaysOfWeek.Wednesday) != 0,
            IsoDayOfWeek.Thursday => (days & ScheduledDaysOfWeek.Thursday) != 0,
            IsoDayOfWeek.Friday => (days & ScheduledDaysOfWeek.Friday) != 0,
            IsoDayOfWeek.Saturday => (days & ScheduledDaysOfWeek.Saturday) != 0,
            IsoDayOfWeek.Sunday => (days & ScheduledDaysOfWeek.Sunday) != 0,
            _ => false,
        };

    /// <inheritdoc/>
    /// <returns><see langword="true"/> if the day of the week in <paramref name="dateTime"/> corresponds to one of the flags in <see cref="DaysOfWeek"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public bool GetStateAt(LocalDateTime dateTime)
        => IsScheduledDayOfWeek(dateTime.DayOfWeek, DaysOfWeek);
}
