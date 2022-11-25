// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using NodaTime;

namespace AaaLib.Scheduling;

/// <summary>
/// Represents an object that determines an on / off state based on a schedule.
/// </summary>
public interface ISchedule
{
    /// <summary>
    /// Determines the scheduled state at a given date and time.
    /// </summary>
    /// <param name="dateTime">The date and time for which the schedule is being queried.</param>
    /// <returns>
    /// <see langword="true"/> if the controlled object should be active at <paramref name="dateTime"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    bool GetStateAt(LocalDateTime dateTime);
}
