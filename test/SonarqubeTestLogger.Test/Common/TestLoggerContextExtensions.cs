using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace SonarqubeTestLogger.Test.Common;

internal static class TestLoggerContextExtensions
{
    public static void SimulateTestRun(
        this TestLoggerContext context,
        params Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult[] testResults
    )
    {
        foreach (var testResult in testResults)
            context.HandleTestResult(new TestResultEventArgs(testResult));

        context.HandleTestRunComplete(
            new TestRunCompleteEventArgs(
                new TestRunStatistics(
                    new Dictionary<TestOutcome, long>
                    {
                        [TestOutcome.Passed] = testResults.Count(r =>
                            r.Outcome == TestOutcome.Passed
                        ),
                        [TestOutcome.Failed] = testResults.Count(r =>
                            r.Outcome == TestOutcome.Failed
                        ),
                        [TestOutcome.Skipped] = testResults.Count(r =>
                            r.Outcome == TestOutcome.Skipped
                        ),
                        [TestOutcome.None] = testResults.Count(r => r.Outcome == TestOutcome.None)
                    }
                ),
                false,
                false,
                null,
                [],
                TimeSpan.FromSeconds(5)
            )
        );
    }
}
