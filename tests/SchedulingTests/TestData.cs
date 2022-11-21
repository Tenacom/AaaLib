// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

public static class TestData
{
    public static readonly IReadOnlyList<LocalDateTime> DateTimes = new List<LocalDateTime>
    {
        new(2022, 5, 13, 16, 5, 19), // Friday
        new(1970, 12, 24, 3, 41, 24), // Thursday
        new(1984, 7, 1, 1, 53, 52), // Sunday
        new(2049, 2, 27, 19, 54, 31), // Saturday
        new(1997, 11, 3, 21, 24, 43), // Monday
        new(2026, 8, 18, 7, 11, 3), // Tuesday
        new(2003, 4, 5, 13, 37, 12), // Wednesday
    };

    public static readonly IReadOnlyList<LocalDate> Dates = DateTimes.Select(dt => dt.Date).ToArray();

    public static IEnumerable<object[]> GetDateTimes()
        => DateTimes.Select(static dt => new object[] { dt });

    public static IEnumerable<object[]> GetDates()
        => DateTimes.Select(static dt => new object[] { dt.Date });

    public static IEnumerable<(bool First, bool Second, bool Result)> GetTwoIntersectedStates()
        => from x in GetOneState()
           from y in GetOneState()
           select (x, y, x && y);

    public static IEnumerable<(bool First, bool Second, bool Third, bool Result)> GetThreeIntersectedStates()
        => from x in GetOneState()
           from y in GetOneState()
           from z in GetOneState()
           select (x, y, z, x && y && z);

    public static IEnumerable<(bool First, bool Second, bool Third, bool Fourth, bool Result)> GetFourIntersectedStates()
        => from x in GetOneState()
           from y in GetOneState()
           from z in GetOneState()
           from k in GetOneState()
           select (x, y, z, k, x && y && z && k);

    public static IEnumerable<(bool First, bool Second, bool Result)> GetTwoCombinedStates()
        => from x in GetOneState()
           from y in GetOneState()
           select (x, y, x || y);

    public static IEnumerable<(bool First, bool Second, bool Third, bool Result)> GetThreeCombinedStates()
        => from x in GetOneState()
           from y in GetOneState()
           from z in GetOneState()
           select (x, y, z, x || y || z);

    public static IEnumerable<(bool First, bool Second, bool Third, bool Fourth, bool Result)> GetFourCombinedStates()
        => from x in GetOneState()
           from y in GetOneState()
           from z in GetOneState()
           from k in GetOneState()
           select (x, y, z, k, x || y || z || k);

    public static IEnumerable<bool> GetOneState()
    {
        yield return false;
        yield return true;
    }

    private static IEnumerable<LocalTime> GetStartOfEveryQuarter()
    {
        var time = LocalTime.MinValue;
        while (time < LocalTime.MaxValue)
        {
            yield return time;
            time = time.PlusMinutes(15);
        }
    }
}
