// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleEnumerableExtensionsTests
{
    public sealed class GetIntersectedStateAt
    {
        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithNullEnumerable_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            IEnumerable<ISchedule> schedules = null!;
            var act = () => schedules.GetIntersectedStateAt(dateTime);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("source");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithEnumerableContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
        {
            IEnumerable<ISchedule> schedules = [Schedule.Always, Schedule.Always, null!];
            var act = () => schedules.GetIntersectedStateAt(dateTime);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("source");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithEmptyEnumerable_ReturnsTrue(LocalDateTime dateTime)
        {
            var items = Enumerable.Empty<ISchedule>();
            items.GetIntersectedStateAt(dateTime).Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithOneItem_ReturnsSameStateAsItem(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var state in TestData.OneState)
                {
                    var schedule = Schedule.GetConstantSchedule(state);
                    var items = new[] { schedule };
                    items.GetIntersectedStateAt(dateTime).Should().Be(state);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithMoreThanOneItem_ReturnsIntersectedStateOfItems(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, thirdState, result) in TestData.ThreeIntersectedStates)
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
}
