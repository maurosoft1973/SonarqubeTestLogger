using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Text;
using System.IO;
using System;

namespace SonarqubeTestLogger;

public class TestLoggerContext(TestLoggerOptions options)
{
    private readonly object _lock = new();
    private readonly List<TestResultInfo> _testResults = [];

    public TestLoggerOptions Options { get; set; } = options;

    public void HandleTestResult(TestResultEventArgs args)
    {
        lock (_lock)
        {
            _testResults.Add(new TestResultInfo(args.Result.DisplayName ?? args.Result.TestCase.DisplayName, args.Result.Outcome, args.Result.Duration.TotalMilliseconds, args.Result.TestCase.FullyQualifiedName, args.Result.TestCase.FullyQualifiedName.ToExtractNamespaceFromFQDN(), args.Result.TestCase.FullyQualifiedName.ToExtractClassFromFQDN(), args.Result.TestCase.FullyQualifiedName.ToExtractMethodFromFQDN()));
        }
    }

    public void HandleTestRunComplete(TestRunCompleteEventArgs _)
    {
        lock (_lock)
        {
            var testResults = _testResults.Where(r => r.Outcome == TestOutcome.Passed || r.Outcome == TestOutcome.Skipped || r.Outcome == TestOutcome.Failed).ToArray();

            var testResultFullyFullyQualifiedNames = from test in testResults
                                                     orderby test.FullyQualifiedName
                                                     select new { Namespace = test.FullyQualifiedName.ToExtractNamespaceFromFQDN(), Class = test.TestClassName, Method = test.TestMethodName };

            if (Options.Verbose)
                foreach (var testResultFullyFullyQualifiedName in testResultFullyFullyQualifiedNames)
                    Console.WriteLine($"Namespace -> {testResultFullyFullyQualifiedName.Namespace}, ClassName -> {testResultFullyFullyQualifiedName.Class}, MethodName -> {testResultFullyFullyQualifiedName.Method}");

            var sourceFilePath = !string.IsNullOrEmpty(Options.PathTestProject) ? Options.PathTestProject : Environment.CurrentDirectory;

            var subDirectories = Options.PathsExcluded.Split(",").ToArray();

            var files = System.IO.Directory.GetFiles(!string.IsNullOrEmpty(Options.PathTestProject) ? Options.PathTestProject : Environment.CurrentDirectory, "*.cs", System.IO.SearchOption.AllDirectories)
                        .Where(p => !p.ContainsInPath(subDirectories)).ToArray();

            if (files.Length == 0)
                Console.WriteLine($"{TestLogger.FriendlyName} - No source files found for {sourceFilePath}, Skip generation report");

            Dictionary<string, string> results = [];

            foreach (var testResultFullyFullyQualifiedName in testResultFullyFullyQualifiedNames)
            {
                if (results.ContainsKey(testResultFullyFullyQualifiedName.Namespace + testResultFullyFullyQualifiedName.Class))
                    continue;

                var (found, file) = Search(files, testResultFullyFullyQualifiedName.Namespace, testResultFullyFullyQualifiedName.Class, testResultFullyFullyQualifiedName.Method);

                if (found && !results.ContainsKey(testResultFullyFullyQualifiedName.Namespace + testResultFullyFullyQualifiedName.Class))
                {
                    if (Options.Verbose)
                        Console.WriteLine($"{TestLogger.FriendlyName} - Found {0} into {1}", testResultFullyFullyQualifiedName.Namespace, file);

                    results.Add(testResultFullyFullyQualifiedName.Namespace + testResultFullyFullyQualifiedName.Class, file);
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<testExecutions version=\"1\">");

            foreach (var result in results)
            {
                var tests = testResults.Where(tr => tr.TestNamespace + tr.TestClassName == result.Key).ToList();

                if (tests.Any())
                {
                    sb.AppendLine(string.Format("{0}{1}", "".PadLeft(4), $"<file path=\"{result.Value}\">"));

                    foreach (var test in tests)
                    {
                        switch (test.Outcome)
                        {
                            case TestOutcome.Passed:
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(8), $"<testCase name=\"{test.DisplayName}\" duration=\"{Math.Round(test.Duration, 0)}\" />"));
                                break;

                            case TestOutcome.Skipped:
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(8), $"<testCase name=\"{test.DisplayName}\" duration=\"{Math.Round(test.Duration, 0)}\">"));
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(12), "<skipped/>"));
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(8), "</testCase>"));
                                break;

                            case TestOutcome.Failed:
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(8), $"<testCase name=\"{test.DisplayName}\" duration=\"{Math.Round(test.Duration, 0)}\">"));
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(12), "<failure/>"));
                                sb.AppendLine(string.Format("{0}{1}", "".PadLeft(8), "</testCase>"));
                                break;
                        }
                    }

                    sb.AppendLine(string.Format("{0}{1}", "".PadLeft(4), "</file>"));
                }
            }

            sb.AppendLine("</testExecutions>");

            var outputFilePath = Path.GetFullPath(Options.LogFilePath);

            System.IO.File.WriteAllText(outputFilePath, sb.ToString());
        }

        (bool found, string file) Search(string[] files, string namespaceName, string className, string methodName)
        {
            bool foundNamespace = false;
            bool foundClass = false;
            bool foundMethod = false;
            string fileName = "";

            foreach (var file in files)
            {
                if (Options.Verbose)
                    Console.WriteLine($"{TestLogger.FriendlyName} - Process Source File -> {file}");

                string[] sourceLinesFile = System.IO.File.ReadAllLines(file);

                sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(namespaceName), (sourceLineFile, i) =>
                {
                    if (Options.Verbose)
                        Console.WriteLine($"{TestLogger.FriendlyName} - Found Namespace {namespaceName} into file {file} at line {i}");

                    foundNamespace = true;
                    fileName = file;
                }, true);

                if (foundNamespace)
                {
                    sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(className), (sourceLineFile, i) =>
                    {
                        if (Options.Verbose)
                            Console.WriteLine($"{TestLogger.FriendlyName} - Found Class {className} into file {file} at line {i}");

                        foundClass = true;
                    }, true);

                    if (foundClass)
                        sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(methodName), (sourceLineFile, i) =>
                        {
                            if (Options.Verbose)
                                Console.WriteLine($"{TestLogger.FriendlyName} - Found Method {methodName} into file {file} at line {i}");

                            foundMethod = true;
                        }, true);
                }

                if (foundNamespace && foundClass && foundMethod)
                    break;
            }

            return (foundNamespace && foundClass && foundMethod, fileName);
        }
    }

    public record TestResultInfo(string DisplayName, TestOutcome Outcome, double Duration, string FullyQualifiedName, string TestNamespace, string TestClassName, string TestMethodName);
}
