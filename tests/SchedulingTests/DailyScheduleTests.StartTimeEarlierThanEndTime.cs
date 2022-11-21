// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class DailyScheduleTests
{
    public sealed class StartTimeEarlierThanEndTime
    {
        private static readonly LocalTime StartTime = LocalTime.FromHoursSinceMidnight(8);
        private static readonly LocalTime EndTime = LocalTime.FromHoursSinceMidnight(20);
        private static readonly DailySchedule TestSchedule = new(StartTime, EndTime);
        private static readonly LocalDate TestDate = TestData.Dates[0];

        [Fact]
        public void GetStateAt_WithTimeEarlierThanStart_ReturnsFalse()
        {
            var time = StartTime.PlusMinutes(-30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }

        [Fact]
        public void GetStateAt_WithTimeEqualToStart_ReturnsTrue()
        {
            var time = StartTime;
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeTrue();
        }

        [Fact]
        public void GetStateAt_WithTimeBetweenStartAndEnd_ReturnsTrue()
        {
            var time = StartTime.PlusMinutes(30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeTrue();
        }

        [Fact]
        public void GetStateAt_WithTimeEqualToEnd_ReturnsFalse()
        {
            var time = EndTime;
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }

        [Fact]
        public void GetStateAt_WithTimeLaterThanEnd_ReturnsFalse()
        {
            var time = EndTime.PlusMinutes(30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }
    }
}
