// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class DailyScheduleTests
{
    public sealed class StartTimeEqualToEndTime
    {
        private static readonly LocalTime StartTime = LocalTime.FromHoursSinceMidnight(12);
        private static readonly DailySchedule TestSchedule = new(StartTime, StartTime);
        private static readonly LocalDate TestDate = TestData.Dates[0];

        [Fact]
        public void GetStateAt_WithTimeEarlierThanStart_ReturnsFalse()
        {
            var time = StartTime.PlusMinutes(-30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }

        [Fact]
        public void GetStateAt_WithTimeEqualToStart_ReturnsFalse()
        {
            var time = StartTime;
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }

        [Fact]
        public void GetStateAt_WithTimeLaterThanStart_ReturnsFalse()
        {
            var time = StartTime.PlusMinutes(30);
            var dateTime = TestDate.At(time);
            TestSchedule.GetStateAt(dateTime).Should().BeFalse();
        }
    }
}
