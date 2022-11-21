// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleEnumerableExtensionsTests
{
    public sealed class GetCombinedStateAt
    {
        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithNullEnumerable_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            IEnumerable<ISchedule> schedules = null!;
            var act = () => schedules.GetCombinedStateAt(dateTime);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("source");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithEnumerableContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
        {
            IEnumerable<ISchedule> schedules = new[] { Schedule.Never, Schedule.Never, null! };
            var act = () => schedules.GetCombinedStateAt(dateTime);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("source");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithEmptyEnumerable_ReturnsFalse(LocalDateTime dateTime)
        {
            var items = Enumerable.Empty<ISchedule>();
            items.GetCombinedStateAt(dateTime).Should().Be(false);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithOneItem_ReturnsSameStateAsItem(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var state in TestData.GetOneState())
                {
                    var schedule = Schedule.GetConstantSchedule(state);
                    var items = new[] { schedule };
                    items.GetCombinedStateAt(dateTime).Should().Be(state);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithMoreThanOneItem_ReturnsCombinedStateOfItems(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, thirdState, result) in TestData.GetThreeCombinedStates())
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    var third = Schedule.GetConstantSchedule(thirdState);
                    var items = new[] { first, second, third };
                    items.GetCombinedStateAt(dateTime).Should().Be(result);
                }
            }
        }
    }
}
