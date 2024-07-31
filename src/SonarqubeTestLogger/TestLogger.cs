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

    public TestLoggerContext Context { get; private set; }

    private void Initialize(TestLoggerEvents events, TestLoggerOptions options)
    {
        if (options.Verbose)
        {
            Console.WriteLine($"{FriendlyName} - Start");
            Console.WriteLine($"{FriendlyName} - LogFilePath      -> {options.LogFilePath}");
            Console.WriteLine($"{FriendlyName} - OutputFilePath   -> {Path.GetFullPath(options.LogFilePath)}");
            Console.WriteLine($"{FriendlyName} - PathSourcesTest: -> {(string.IsNullOrEmpty(options.PathTestProject) ? options.PathTestProject : Environment.CurrentDirectory)}");
        }

        var context = new TestLoggerContext(options);

        events.TestResult += (_, args) => context.HandleTestResult(args);
        events.TestRunComplete += (_, args) => context.HandleTestRunComplete(args);

        Context = context;
    }

    public void Initialize(TestLoggerEvents events, string testRunDirectory) =>
        Initialize(events, new TestLoggerOptions() { LogFilePath = Path.Combine(testRunDirectory, "SonarQubeReportTest.xml") });

    public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters) =>
        Initialize(events, TestLoggerOptions.Resolve(parameters));
}
