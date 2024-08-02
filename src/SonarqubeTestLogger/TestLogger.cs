using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace SonarqubeTestLogger;

[FriendlyName(FriendlyName)]
[ExtensionUri("logger://Microsoft/TestPlatform/SonarQubeReportTestXmlLogger/v1")]
public class TestLogger : ITestLoggerWithParameters
{
    public const string FriendlyName = "sonarqube";

    public TestLoggerContext? Context { get; private set; }

    public const string DefaultTestResultFile = "SonarqubeReportTest.xml";

    public IConsoleK ConsoleK { get; set; } = new ConsoleK();

    private void Initialize(TestLoggerEvents events, TestLoggerOptions options)
    {
        if (options.Verbose)
        {
            ConsoleK.WriteLine($"{FriendlyName} - Start");
            ConsoleK.WriteLine($"{FriendlyName} - LogFileName      -> {options.LogFileName}");
            ConsoleK.WriteLine($"{FriendlyName} - LogFilePath      -> {options.LogFilePath}");
            ConsoleK.WriteLine($"{FriendlyName} - TestRunDirectory -> {options.TestRunDirectory}");
            ConsoleK.WriteLine($"{FriendlyName} - PathSourcesTest: -> {(string.IsNullOrEmpty(options.PathTestProject) ? options.PathTestProject : Environment.CurrentDirectory)}");
        }

        var context = new TestLoggerContext(options, ConsoleK);

        events.TestResult += (_, args) => context.HandleTestResult(args);
        events.TestRunComplete += (_, args) => context.HandleTestRunComplete(args);

        Context = context;
    }

    public void Initialize(TestLoggerEvents events, string testRunDirectory)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        if (testRunDirectory == null)
            throw new ArgumentNullException(nameof(testRunDirectory));

        var config = new Dictionary<string, string?>
        {
            { DefaultLoggerParameterNames.TestRunDirectory, testRunDirectory },
            { DefaultLoggerParameterNames.TargetFramework, "" },
        };

        Initialize(events, config);
    }

    public void Initialize(TestLoggerEvents events, Dictionary<string, string?> parameters)
    {
        Initialize(events, TestLoggerOptions.Resolve(parameters));
    }
}
