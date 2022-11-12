// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace AaaLibTests;

public class AaaTest
{
    [Fact]
    public void GetTheAnswer_ReturnsTheRealAnswer()
    {
        Aaa.GetTheAnswer().Should().Be(42, "because that's what Douglas Adams said");
    }

    [Fact]
    public void GetTheAnswerInItalian_ReturnsTheRealAnswer()
    {
        Aaa.GetTheAnswerInItalian().ToString().Should().Be("Quarantadue", "because that's what Douglas Adams would have said if he had been Italian");
    }
}
