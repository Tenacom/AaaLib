// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class ScheduleTests
{
    public sealed class GetCombinedStateAt
    {
        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithTwoParams_WhenFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetCombinedStateAt(dateTime, null!, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("first");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithTwoParams_WhenSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("second");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithTwoParams_ReturnsCombinedStateOfParams(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, result) in TestData.TwoCombinedStates)
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    Schedule.GetCombinedStateAt(dateTime, first, second).Should().Be(result);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenFirstIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetCombinedStateAt(dateTime, null!, Schedule.Always, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("first");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenSecondIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, null!, Schedule.Always);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("second");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithThreeParams_WhenThirdIsNull_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            var act = () => Schedule.GetCombinedStateAt(dateTime, Schedule.Always, Schedule.Always, null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("third");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithThreeParams_ReturnsCombinedStateOfParams(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, thirdState, result) in TestData.ThreeCombinedStates)
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    var third = Schedule.GetConstantSchedule(thirdState);
                    Schedule.GetCombinedStateAt(dateTime, first, second, third).Should().Be(result);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithNullParamArray_ThrowsArgumentNullException(LocalDateTime dateTime)
        {
            ISchedule[] schedules = null!;
            var act = () => Schedule.GetCombinedStateAt(dateTime, schedules);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("schedules");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithParamArrayContainingNull_ThrowsArgumentException(LocalDateTime dateTime)
        {
            var schedules = new ISchedule[] { Schedule.Never, Schedule.Never, null! };
            var act = () => Schedule.GetCombinedStateAt(dateTime, schedules);
            act.Should().Throw<ArgumentException>()
                .WithParameterName("schedules");
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithEmptyParamArray_ReturnsFalse(LocalDateTime dateTime)
        {
            Schedule.GetCombinedStateAt(dateTime, []).Should().Be(false);
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithParamArrayOfOneItem_ReturnsSameStateAsItem(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var state in TestData.OneState)
                {
                    var schedule = Schedule.GetConstantSchedule(state);
                    Schedule.GetCombinedStateAt(dateTime, schedule).Should().Be(state);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
        public void WithParamArray_ReturnsCombinedStateOfItems(LocalDateTime dateTime)
        {
            using (new AssertionScope())
            {
                foreach (var (firstState, secondState, result) in TestData.TwoCombinedStates)
                {
                    var first = Schedule.GetConstantSchedule(firstState);
                    var second = Schedule.GetConstantSchedule(secondState);
                    Schedule.GetCombinedStateAt(dateTime, [first, second]).Should().Be(result);
                }
            }
        }
    }
}
