// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace AaaLib.Scheduling;

/// <summary>
/// <para>Specifies constants that define the days of the week on which a weekly schedule is enabled.</para>
/// <para>This enumeration supports a bitwise combination of its member values.</para>
/// </summary>
[Flags]
public enum ScheduledDaysOfWeek
{
    /// <summary>
    /// Specifies that a weekly schedule is never enabled.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Mondays.
    /// </summary>
    Monday = 0x01,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Tuesdays.
    /// </summary>
    Tuesday = 0x02,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Wednesdays.
    /// </summary>
    Wednesday = 0x04,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Thursdays.
    /// </summary>
    Thursday = 0x08,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Fridays.
    /// </summary>
    Friday = 0x10,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Saturdays.
    /// </summary>
    Saturday = 0x20,

    /// <summary>
    /// Specifies that a weekly schedule is enabled on Sundays.
    /// </summary>
    Sunday = 0x40,

    /// <summary>
    /// Specifies that a weekly schedule is always enabled.
    /// </summary>
    All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
}
