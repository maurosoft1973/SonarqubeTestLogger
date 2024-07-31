using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace SonarqubeTestLogger.Test.Common;

internal class TestResultBuilder
{
    private Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult _testResult =
        new(
            new TestCase
            {
                Id = Guid.NewGuid(),
                Source = "FakeTests.dll",
                FullyQualifiedName = "FakeTests.FakeTest",
                DisplayName = "FakeTest",
            }
        );

    public TestResultBuilder SetDisplayName(string displayName)
    {
        _testResult.TestCase.DisplayName = displayName;
        return this;
    }

    public TestResultBuilder SetDuration(double millisecond)
    {
        _testResult.Duration = TimeSpan.FromMilliseconds(millisecond);
        return this;
    }

    public TestResultBuilder SetFullyQualifiedName(string fullyQualifiedName)
    {
        _testResult.TestCase.FullyQualifiedName = fullyQualifiedName;
        return this;
    }

    public TestResultBuilder SetTrait(string name, string value)
    {
        _testResult.TestCase.Traits.Add(name, value);
        return this;
    }

    public TestResultBuilder SetOutcome(TestOutcome outcome)
    {
        _testResult.Outcome = outcome;
        return this;
    }

    public TestResultBuilder SetErrorMessage(string message)
    {
        _testResult.ErrorMessage = message;
        return this;
    }

    public TestResultBuilder SetErrorStackTrace(string stackTrace)
    {
        _testResult.ErrorStackTrace = stackTrace;
        return this;
    }

    public Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult Build()
    {
        var testResult = _testResult;
        _testResult = new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(
            new TestCase()
        );

        return testResult;
    }
}
