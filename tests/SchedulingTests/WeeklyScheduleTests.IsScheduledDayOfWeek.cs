// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

partial class WeeklyScheduleTests
{
    public sealed class IsScheduledDayOfWeek
    {
        [Theory]
        [MemberData(nameof(TestData.SingleDaysOfWeekData), MemberType = typeof(TestData))]
        public void WithSingleDay_WithScheduledDay_ReturnsTrue(IsoDayOfWeek dayOfWeek, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            WeeklySchedule.IsScheduledDayOfWeek(dayOfWeek, scheduledDaysOfWeek).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestData.SingleDaysOfWeekData), MemberType = typeof(TestData))]
        public void WithSingleDay_WithNonScheduledDay_ReturnsFalse(IsoDayOfWeek dayOfWeek, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            using (new AssertionScope())
            {
                var otherDays = TestData.SingleDaysOfWeek
                    .Select(x => x.IsoDay)
                    .Where(x => x != dayOfWeek);

                foreach (var otherDay in otherDays)
                {
                    WeeklySchedule.IsScheduledDayOfWeek(otherDay, scheduledDaysOfWeek).Should().BeFalse();
                }

                WeeklySchedule.IsScheduledDayOfWeek(IsoDayOfWeek.None, scheduledDaysOfWeek).Should().BeFalse();
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DaysOfWeekInPairsData), MemberType = typeof(TestData))]
        public void WithMultipleDays_WithScheduledDay_ReturnsTrue(IsoDayOfWeek dayOfWeek1, IsoDayOfWeek dayOfWeek2, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            using (new AssertionScope())
            {
                WeeklySchedule.IsScheduledDayOfWeek(dayOfWeek1, scheduledDaysOfWeek).Should().BeTrue();
                WeeklySchedule.IsScheduledDayOfWeek(dayOfWeek2, scheduledDaysOfWeek).Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(TestData.DaysOfWeekInPairsData), MemberType = typeof(TestData))]
        public void WithMultipleDays_WithNonScheduledDay_ReturnsFalse(IsoDayOfWeek dayOfWeek1, IsoDayOfWeek dayOfWeek2, ScheduledDaysOfWeek scheduledDaysOfWeek)
        {
            using (new AssertionScope())
            {
                var otherDays = TestData.SingleDaysOfWeek
                    .Select(x => x.IsoDay)
                    .Where(x => x != dayOfWeek1 && x != dayOfWeek2);

                foreach (var otherDay in otherDays)
                {
                    WeeklySchedule.IsScheduledDayOfWeek(otherDay, scheduledDaysOfWeek).Should().BeFalse();
                }

                WeeklySchedule.IsScheduledDayOfWeek(IsoDayOfWeek.None, scheduledDaysOfWeek).Should().BeFalse();
            }
        }
    }
}
