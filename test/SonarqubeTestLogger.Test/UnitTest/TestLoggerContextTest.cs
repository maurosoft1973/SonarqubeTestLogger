using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SonarqubeTestLogger.Test.Common;

namespace SonarqubeTestLogger.Test.UnitTest;

[TestClass]
public class TestLoggerContextTest
{
    [TestInitialize]
    public void TestInitialize()
    {
        var source1 =
            @"
            using Microsoft.VisualStudio.TestPlatform.ObjectModel;

            namespace TestProject.UnitTest
            {
                public class Test1
                {
                    public void TestMethod1()
                    {
                    }
                }
            }
            ";

        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "source1.cs"), source1);

        var source2 =
            @"
            using Microsoft.VisualStudio.TestPlatform.ObjectModel;

            namespace TestProject.UnitTest
            {
                public class Test2
                {
                    public void TestMethod2()
                    {
                    }

                    public void TestMethod3()
                    {
                    }
                }
            }
            ";

        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "source2.cs"), source2);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "source1.cs")))
            File.Delete(Path.Combine(AppContext.BaseDirectory, "source1.cs"));

        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "source2.cs")))
            File.Delete(Path.Combine(AppContext.BaseDirectory, "source2.cs"));

        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "SonarqubeReportTest.xml")))
            File.Delete(Path.Combine(AppContext.BaseDirectory, "SonarqubeReportTest.xml"));
    }

    [TestMethod]
    public void TestLoggerContext_Should_Return_The_Xml_Report_When_Tests_Are_Present()
    {
        //Arrange
        var sonarqubeTestReportExpected =
            @$"<?xml version=""1.0"" encoding=""utf-8""?>
        <testExecutions version=""1"">
            <file path=""{Path.Combine(AppContext.BaseDirectory, "source1.cs")}"">
                <testCase name=""TestMethod1"" duration=""4001"">
                    <skipped/>
                </testCase>
            </file>
            <file path=""{Path.Combine(AppContext.BaseDirectory, "source2.cs")}"">
                <testCase name=""TestMethod2"" duration=""2100"" />
                <testCase name=""TestMethod3"" duration=""1500"">
                    <failure/>
                </testCase>
            </file>
        </testExecutions>";

        var docExpected = new XmlDocument();
        docExpected.LoadXml(sonarqubeTestReportExpected);
        var consoleKTest = new ConsoleKTest();

        var context = new TestLoggerContext(
            new TestLoggerOptions()
            {
                LogFilePath = "SonarqubeReportTest.xml",
                PathTestProject = AppContext.BaseDirectory,
                PathsExcluded = "obj",
                Verbose = true
            },
            consoleKTest
        );

        var test1 = new TestResultBuilder()
            .SetDisplayName("TestMethod1")
            .SetFullyQualifiedName("TestProject.UnitTest.Test1.TestMethod1")
            .SetOutcome(TestOutcome.Skipped)
            .SetDuration(4001)
            .Build();

        var test2 = new TestResultBuilder()
            .SetDisplayName("TestMethod2")
            .SetFullyQualifiedName("TestProject.UnitTest.Test2.TestMethod2")
            .SetOutcome(TestOutcome.Passed)
            .SetDuration(2100)
            .Build();

        var test3 = new TestResultBuilder()
            .SetDisplayName("TestMethod3")
            .SetFullyQualifiedName("TestProject.UnitTest.Test2.TestMethod3")
            .SetOutcome(TestOutcome.Failed)
            .SetDuration(1500)
            .Build();

        //Act
        context.SimulateTestRun(test1, test2, test3);

        //Assert
        var sourceSonarqubeReportsActual = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "SonarqubeReportTest.xml"));
        var docActual = new XmlDocument();
        docActual.LoadXml(sourceSonarqubeReportsActual);

        Assert.AreEqual(docExpected.OuterXml, docActual.OuterXml);
    }

    public class ConsoleKTest : IConsoleK
    {
        private readonly StringBuilder sb = new StringBuilder();

        public void WriteLine(string message) => sb.AppendLine(message);

        public string[] Messages() => sb.ToString().Split('\n');
    }
}
