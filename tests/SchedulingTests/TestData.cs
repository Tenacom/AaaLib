// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

public static class TestData
{
    public static readonly IEnumerable<bool> OneState = [false, true];

    public static readonly IEnumerable<(bool First, bool Second, bool Result)> TwoIntersectedStates
        = from x in OneState
          from y in OneState
          select (x, y, x && y);

    public static readonly IEnumerable<(bool First, bool Second, bool Third, bool Result)> ThreeIntersectedStates
        = from x in OneState
          from y in OneState
          from z in OneState
          select (x, y, z, x && y && z);

    public static readonly IEnumerable<(bool First, bool Second, bool Result)> TwoCombinedStates
        = from x in OneState
          from y in OneState
          select (x, y, x || y);

    public static readonly IEnumerable<(bool First, bool Second, bool Third, bool Result)> ThreeCombinedStates
        = from x in OneState
          from y in OneState
          from z in OneState
          select (x, y, z, x || y || z);

    public static readonly IReadOnlyList<LocalDateTime> DateTimes = [
        new(2022, 5, 13, 16, 5, 19), // Friday
        new(1970, 12, 24, 3, 41, 24), // Thursday
        new(1984, 7, 1, 1, 53, 52), // Sunday
        new(2049, 2, 27, 19, 54, 31), // Saturday
        new(1997, 11, 3, 21, 24, 43), // Monday
        new(2026, 8, 18, 7, 11, 3), // Tuesday
        new(2003, 4, 5, 13, 37, 12), // Wednesday
    ];

    public static readonly IReadOnlyList<(IsoDayOfWeek IsoDay, ScheduledDaysOfWeek ScheduledDays)> SingleDaysOfWeek = [
        (IsoDayOfWeek.Monday, ScheduledDaysOfWeek.Monday),
        (IsoDayOfWeek.Tuesday, ScheduledDaysOfWeek.Tuesday),
        (IsoDayOfWeek.Wednesday, ScheduledDaysOfWeek.Wednesday),
        (IsoDayOfWeek.Thursday, ScheduledDaysOfWeek.Thursday),
        (IsoDayOfWeek.Friday, ScheduledDaysOfWeek.Friday),
        (IsoDayOfWeek.Saturday, ScheduledDaysOfWeek.Saturday),
        (IsoDayOfWeek.Sunday, ScheduledDaysOfWeek.Sunday),
    ];

    public static readonly IReadOnlyList<(IsoDayOfWeek IsoDay1, IsoDayOfWeek IsoDay2, ScheduledDaysOfWeek ScheduledDays)> DaysOfWeekInPairs
        = (from x in SingleDaysOfWeek
          from y in SingleDaysOfWeek
          select (x.IsoDay, y.IsoDay, x.ScheduledDays | y.ScheduledDays)).ToArray();

    public static readonly IReadOnlyList<LocalDate> Dates = DateTimes.Select(dt => dt.Date).ToArray();

    public static readonly TheoryData<LocalDateTime> DateTimesData
        = new(DateTimes);

    public static readonly TheoryData<IsoDayOfWeek, ScheduledDaysOfWeek> SingleDaysOfWeekData
        = SingleDaysOfWeek.Aggregate(
            new TheoryData<IsoDayOfWeek, ScheduledDaysOfWeek>(),
            (x, d) =>
            {
                x.Add(d.IsoDay, d.ScheduledDays);
                return x;
            });

    public static readonly TheoryData<IsoDayOfWeek, IsoDayOfWeek, ScheduledDaysOfWeek> DaysOfWeekInPairsData
        = DaysOfWeekInPairs.Aggregate(
            new TheoryData<IsoDayOfWeek, IsoDayOfWeek, ScheduledDaysOfWeek>(),
            (x, d) =>
            {
                x.Add(d.IsoDay1, d.IsoDay2, d.ScheduledDays);
                return x;
            });
}
