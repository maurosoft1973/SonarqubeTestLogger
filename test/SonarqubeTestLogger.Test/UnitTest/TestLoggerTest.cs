using FluentAssertions;
using SonarqubeTestLogger.Test.Common;
using System.Xml;

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

    [TestMethod]
    public void TestLogger_Should_Return_The_Empty_Xml_Report_When_Tests_Not_Found()
    {
        //Arrange
        var sonarqubeTestReportExpected = @$"<?xml version=""1.0"" encoding=""utf-8""?>
<testExecutions version=""1"">
</testExecutions>";

        var docExpected = new XmlDocument();
        docExpected.LoadXml(sonarqubeTestReportExpected);

        var testLogger = new TestLogger();
        testLogger.Initialize(new FakeTestLoggerEvents(), AppContext.BaseDirectory);

        //Act
        testLogger.Context?.SimulateTestRun();

        //Assert
        var sourceSonarqubeReportsActual = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "SonarqubeReportTest.xml"));
        var docActual = new XmlDocument();
        docActual.LoadXml(sourceSonarqubeReportsActual);

        Assert.AreEqual(docExpected.OuterXml, docActual.OuterXml);
    }

    [TestMethod]
    public void TestLogger_Should_Use_Default_Report_FileName_When_LogFileName_Not_Set()
    {
        //Arrange
        var testLogger = new TestLogger();
        testLogger.Initialize(new FakeTestLoggerEvents(), []);

        //Act
        testLogger.Context?.SimulateTestRun();

        //Assert
        Assert.AreEqual("", testLogger.Context?.Options.LogFileName);
    }
}
