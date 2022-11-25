// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class DailyScheduleTests
{
    public sealed class StartTimeLaterThanEndTime
    {
        private static readonly LocalTime StartTime = LocalTime.FromHoursSinceMidnight(20);
        private static readonly LocalTime EndTime = LocalTime.FromHoursSinceMidnight(8);
        private static readonly DailySchedule TestSchedule = new(StartTime, EndTime);
        private static readonly LocalDate TestDate = TestData.Dates[0];

        [Fact]
        public void GetStateAt_WithTimeEarlierThanEnd_ReturnsTrue()
        {
            var time = EndTime.PlusMinutes(-30);
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
        public void GetStateAt_WithTimeBetweenEndAndStart_ReturnsFalse()
        {
            var time = EndTime.PlusMinutes(30);
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
        public void GetStateAt_WithTimeLaterThanStart_ReturnsTrue()
        {
            var time = StartTime.PlusMinutes(30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeTrue();
        }
    }
}
