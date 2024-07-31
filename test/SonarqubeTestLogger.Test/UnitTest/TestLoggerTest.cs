using FluentAssertions;
using SonarqubeTestLogger.Test.Common;

namespace SonarqubeTestLogger.Test.UnitTest;

[TestClass]
public class TestLoggerTest
{
    [TestMethod]
    public void TestLogger_Should_Return_Context_Not_Null_When_Initialize()
    {
        // Arrange
        var events = new FakeTestLoggerEvents();
        var logger = new TestLogger();

        // Act
        logger.Initialize(events, Directory.GetCurrentDirectory());

        // Assert
        logger.Context.Should().NotBeNull();
    }
}
