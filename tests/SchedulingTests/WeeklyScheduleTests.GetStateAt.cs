// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

partial class WeeklyScheduleTests
{
    public sealed class GetStateAt
    {
        private static readonly LocalDate TestDate = TestData.Dates[0];

        [Theory]
        [MemberData(nameof(TestData.GetSingleDaysOfWeek), MemberType = typeof(TestData))]
        public void WithSingleDay_WithScheduledDay_ReturnsTrue(IsoDayOfWeek dayOfWeek, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            var schedule = new WeeklySchedule(scheduledDaysOfWeek);
            var dateTime = TestDate.Next(dayOfWeek).At(LocalTime.Noon);
            schedule.GetStateAt(dateTime).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestData.GetSingleDaysOfWeek), MemberType = typeof(TestData))]
        public void WithSingleDay_WithNonScheduledDay_ReturnsFalse(IsoDayOfWeek dayOfWeek, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            using (new AssertionScope())
            {
                var schedule = new WeeklySchedule(scheduledDaysOfWeek);
                var otherDays = TestData.SingleDaysOfWeek
                    .Select(x => x.IsoDay)
                    .Where(x => x != dayOfWeek);

                foreach (var otherDay in otherDays)
                {
                    var dateTime = TestDate.Next(otherDay).At(LocalTime.Noon);
                    schedule.GetStateAt(dateTime).Should().BeFalse();
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDaysOfWeekInPairs), MemberType = typeof(TestData))]
        public void WithMultipleDays_WithScheduledDay_ReturnsTrue(IsoDayOfWeek dayOfWeek1, IsoDayOfWeek dayOfWeek2, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            var schedule = new WeeklySchedule(scheduledDaysOfWeek);
            using (new AssertionScope())
            {
                var dateTime1 = TestDate.Next(dayOfWeek1).At(LocalTime.Noon);
                var dateTime2 = TestDate.Next(dayOfWeek2).At(LocalTime.Noon);
                schedule.GetStateAt(dateTime1).Should().BeTrue();
                schedule.GetStateAt(dateTime2).Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(TestData.GetDaysOfWeekInPairs), MemberType = typeof(TestData))]
        public void WithMultipleDays_WithNonScheduledDay_ReturnsFalse(IsoDayOfWeek dayOfWeek1, IsoDayOfWeek dayOfWeek2, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            var schedule = new WeeklySchedule(scheduledDaysOfWeek);
            using (new AssertionScope())
            {
                var otherDays = TestData.SingleDaysOfWeek
                    .Select(x => x.IsoDay)
                    .Where(x => x != dayOfWeek1 && x != dayOfWeek2);

                foreach (var otherDay in otherDays)
                {
                    var dateTime = TestDate.Next(otherDay).At(LocalTime.Noon);
                    schedule.GetStateAt(dateTime).Should().BeFalse();
                }
            }
        }
    }
}
