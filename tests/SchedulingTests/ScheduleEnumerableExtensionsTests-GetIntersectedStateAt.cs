// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleEnumerableExtensionsTests
{
    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetIntersectedStateAt_WithNullEnumerable_ThrowsArgumentNullException(LocalDateTime dateTime)
    {
        IEnumerable<ISchedule> schedules = null!;
        var act = () => schedules.GetIntersectedStateAt(dateTime);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetIntersectedStateAt_WithEnumerableContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
    {
        IEnumerable<ISchedule> schedules = new[] { Schedule.Always, Schedule.Always, null! };
        var act = () => schedules.GetIntersectedStateAt(dateTime);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("source");
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetIntersectedStateAt_WithEmptyEnumerable_ReturnsTrue(LocalDateTime dateTime)
    {
        var items = Enumerable.Empty<ISchedule>();
        items.GetIntersectedStateAt(dateTime).Should().Be(true);
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetIntersectedStateAt_WithOneItem_ReturnsSameState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var state in TestData.GetOneState())
            {
                var schedule = Schedule.GetConstantSchedule(state);
                var items = new[] { schedule };
                items.GetIntersectedStateAt(dateTime).Should().Be(state);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
    public void GetIntersectedStateAt_WithMoreThanOneItem_ReturnsIntersectedState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var (firstState, secondState, thirdState, result) in TestData.GetThreeIntersectedStates())
            {
                var first = Schedule.GetConstantSchedule(firstState);
                var second = Schedule.GetConstantSchedule(secondState);
                var third = Schedule.GetConstantSchedule(thirdState);
                var items = new[] { first, second, third };
                items.GetIntersectedStateAt(dateTime).Should().Be(result);
            }
        }
    }
}
