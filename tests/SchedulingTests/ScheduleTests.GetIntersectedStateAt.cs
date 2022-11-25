// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleTests
{
    public sealed class GetIntersectedStateAt
    {
        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithTwoParams_WhenFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetIntersectedStateAt(dateTime, null!, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("first");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithTwoParams_WhenSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetIntersectedStateAt(dateTime, Schedule.Always, null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("second");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithTwoParams_ReturnsIntersectedStateOfParams(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, result) in TestData.GetTwoIntersectedStates())
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    Schedule.GetIntersectedStateAt(dateTime, first, second).Should().Be(result);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetIntersectedStateAt(dateTime, null!, Schedule.Always, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("first");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetIntersectedStateAt(dateTime, Schedule.Always, null!, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("second");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenThirdIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetIntersectedStateAt(dateTime, Schedule.Always, Schedule.Always, null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("third");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithThreeParams_ReturnsIntersectedStateOfParams(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, thirdState, result) in TestData.GetThreeIntersectedStates())
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    var third = Schedule.GetConstantSchedule(thirdState);
                    Schedule.GetIntersectedStateAt(dateTime, first, second, third).Should().Be(result);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithNullParamArray_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            ISchedule[] schedules = null!;
            var act = () => Schedule.GetIntersectedStateAt(dateTime, schedules);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("schedules");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithParamArrayContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
        {
            var schedules = new ISchedule[] { Schedule.Always, Schedule.Always, null! };
            var act = () => Schedule.GetIntersectedStateAt(dateTime, schedules);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("schedules");
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithEmptyParamArray_ReturnsTrue(LocalDateTime dateTime)
        {
            Schedule.GetIntersectedStateAt(dateTime, Array.Empty<ISchedule>()).Should().Be(true);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithParamArrayOfOneItem_ReturnsSameStateAsItem(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var state in TestData.GetOneState())
                {
                    var schedule = Schedule.GetConstantSchedule(state);
                    Schedule.GetIntersectedStateAt(dateTime, schedule).Should().Be(state);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDateTimes), MemberType = typeof(TestData))]
        public void WithParamArray_ReturnsIntersectedStateOfItems(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, result) in TestData.GetTwoIntersectedStates())
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    Schedule.GetIntersectedStateAt(dateTime, new[] { first, second }).Should().Be(result);
                }
            }
        }
    }
}
