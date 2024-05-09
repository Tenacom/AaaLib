// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using NodaTime;

namespace AaaLib.Scheduling;

partial class Schedule
{
    private sealed class AlwaysSchedule : ISchedule
    {
        public bool GetStateAt(LocalDateTime dateTime) => true;
    }
}
