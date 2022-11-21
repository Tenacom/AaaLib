// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleTests
{
    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithTwoSchedulesFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        var act = () => Schedule.GetCombinedStateAt(dateTime, null!, Schedule.Always);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("first");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithTwoSchedulesSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("second");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithTwoSchedules_ReturnsCombinedState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var (firstState, secondState, result) in TestData.GetTwoCombinedStates())
            {
                var first = Schedule.GetConstantSchedule(firstState);
                var second = Schedule.GetConstantSchedule(secondState);
                Schedule.GetCombinedStateAt(dateTime, first, second).Should().Be(result);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithThreeSchedulesFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        var act = () => Schedule.GetCombinedStateAt(dateTime, null!, Schedule.Always, Schedule.Always);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("first");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithThreeSchedulesSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, null!, Schedule.Always);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("second");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithThreeSchedulesThirdIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, Schedule.Always, null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("third");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithThreeSchedules_ReturnsCombinedState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var (firstState, secondState, thirdState, result) in TestData.GetThreeCombinedStates())
            {
                var first = Schedule.GetConstantSchedule(firstState);
                var second = Schedule.GetConstantSchedule(secondState);
                var third = Schedule.GetConstantSchedule(thirdState);
                Schedule.GetCombinedStateAt(dateTime, first, second, third).Should().Be(result);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithNullScheduleArray_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        ISchedule[] schedules = null!;
        var act = () => Schedule.GetCombinedStateAt(dateTime, schedules);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("schedules");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithScheduleArrayContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
    {
        var schedules = new ISchedule[] { Schedule.Never, Schedule.Never, null! };
        var act = () => Schedule.GetCombinedStateAt(dateTime, schedules);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("schedules");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithEmptyScheduleArray_ReturnsFalse(LocalDateTime dateTime)
    {
        Schedule.GetCombinedStateAt(dateTime, Array.Empty<ISchedule>()).Should().Be(false);
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithOneSchedule_ReturnsSameState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var state in TestData.GetOneState())
            {
                var schedule = Schedule.GetConstantSchedule(state);
                Schedule.GetCombinedStateAt(dateTime, schedule).Should().Be(state);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetCombinedStateAt_WithScheduleArray_ReturnsCombinedState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var (firstState, secondState, result) in TestData.GetTwoCombinedStates())
            {
                var first = Schedule.GetConstantSchedule(firstState);
                var second = Schedule.GetConstantSchedule(secondState);
                Schedule.GetCombinedStateAt(dateTime, new[] { first, second }).Should().Be(result);
            }
        }
    }
}
