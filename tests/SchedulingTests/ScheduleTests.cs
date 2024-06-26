﻿// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

public sealed partial class ScheduleTests
{
    [Theory]
    [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
    public void Always_GetStateAt_ReturnsTrue(LocalDateTime dateTime)
    {
        var schedule = Schedule.Always;

        schedule.GetStateAt(dateTime).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
    public void Never_GetStateAt_ReturnsFalse(LocalDateTime dateTime)
    {
        var schedule = Schedule.Never;

        schedule.GetStateAt(dateTime).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(TestData.DateTimesData), MemberType = typeof(TestData))]
    public void GetConstantSchedule_GetStateAt_ReturnsSameState(LocalDateTime dateTime)
    {
        using (new AssertionScope())
        {
            foreach (var state in TestData.OneState)
            {
                var schedule = Schedule.GetConstantSchedule(state);
                schedule.GetStateAt(dateTime).Should().Be(state);
            }
        }
    }
}
