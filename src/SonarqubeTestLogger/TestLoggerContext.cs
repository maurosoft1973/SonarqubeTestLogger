using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Text;
using System.IO;
using System;
using System.Reflection;

namespace SonarqubeTestLogger;

public class TestLoggerContext(TestLoggerOptions options, IConsoleK console)
{
    private readonly object _lock = new();
    private readonly List<TestResultInfo> _testResults = [];
    private readonly IConsoleK _console = console;

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
                    _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Found Test with Namespace -> {testResultFullyFullyQualifiedName.Namespace}, ClassName -> {testResultFullyFullyQualifiedName.Class}, MethodName -> {testResultFullyFullyQualifiedName.Method} from TestResults");

            var sourceFilePath = !string.IsNullOrEmpty(Options.PathTestProject) ? Options.PathTestProject : Environment.CurrentDirectory;

            var subDirectories = Options.PathsExcluded.Split(",").ToArray();

            var files = System.IO.Directory.GetFiles(!string.IsNullOrEmpty(Options.PathTestProject) ? Options.PathTestProject : Environment.CurrentDirectory, "*.cs", System.IO.SearchOption.AllDirectories)
                        .Where(p => !p.ContainsInPath(subDirectories)).ToArray();

            if (files.Length == 0)
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - No source files found for {sourceFilePath}, Skip generation report");

            Dictionary<string, string> results = [];

            foreach (var testResultFullyFullyQualifiedName in testResultFullyFullyQualifiedNames)
            {
                var (found, file) = Search(files, testResultFullyFullyQualifiedName.Namespace, testResultFullyFullyQualifiedName.Class, testResultFullyFullyQualifiedName.Method);

                if (found)
                {
                    if (!results.ContainsKey(testResultFullyFullyQualifiedName.Namespace + testResultFullyFullyQualifiedName.Class))
                    {
                        results.Add(testResultFullyFullyQualifiedName.Namespace + testResultFullyFullyQualifiedName.Class, file);
                    }
                }
                else
                {
                    if (Options.Verbose)
                        _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - No test found with namespace:{testResultFullyFullyQualifiedName.Namespace}, classname:{testResultFullyFullyQualifiedName.Class}, methodname: {testResultFullyFullyQualifiedName.Method}");
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<testExecutions version=\"1\">");

            var totalTest = 0;

            foreach (var result in results)
            {
                var tests = testResults.Where(tr => tr.TestNamespace + tr.TestClassName == result.Key).ToList();

                if (tests.Any())
                {
                    sb.AppendLine(string.Format("{0}{1}", "".PadLeft(4), $"<file path=\"{result.Value}\">"));

                    foreach (var test in tests)
                    {
                        totalTest++;

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

            if (Options.Verbose)
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Total Tests: {totalTest}");

            var outputFilePath = "";
            var outputFileName = !string.IsNullOrEmpty(Options.LogFileName) ? Options.LogFileName : TestLogger.DefaultTestResultFile;

            if (Options.LogFilePath != "")
            {
                try
                {
                    var fi = new FileInfo(Options.LogFilePath);
                    var parentDirectory = fi.Directory.FullName;

                    if (!Directory.Exists(parentDirectory))
                    {
                        Directory.CreateDirectory(parentDirectory);
                        if (Options.Verbose)
                            _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Successful directory creation: {parentDirectory}");
                    }

                    outputFilePath = Options.LogFilePath;
                }
                catch (Exception ex)
                {
                    if (Options.Verbose)
                        _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - An error occurred when creating the directory tree {Options.LogFilePath}: {ex.Message}");
                }
            }

            var outputReportFilePath = outputFilePath switch
            {
                "" => Path.Combine(Options.TestRunDirectory, outputFileName),
                _ => Path.GetFullPath(Options.LogFilePath),
            };

            if (File.Exists(outputReportFilePath))
            {
                if (Options.Verbose)
                    _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - The report file {outputReportFilePath} exist, generate new file");

                var tmpOutputBaseReport = Path.GetFileNameWithoutExtension(outputReportFilePath);
                var tmpOutputReportPath = Path.GetDirectoryName(outputReportFilePath);
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - OutputBaseReport {tmpOutputBaseReport}");
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - OutputReportPath {tmpOutputReportPath}");

                var totalFiles = Directory.GetFiles(tmpOutputReportPath, tmpOutputBaseReport + "-*.xml");
                var instance = totalFiles.Length + 1;
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Total Reports Files {totalFiles.Length}");
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Report File Path Rewrite {Path.Combine(tmpOutputReportPath, tmpOutputBaseReport + "-" + instance.ToString("000") + ".xml")}");

                File.WriteAllText(Path.Combine(tmpOutputReportPath, tmpOutputBaseReport + "-" + instance.ToString("000") + ".xml"), sb.ToString());
            }
            else
                File.WriteAllText(outputReportFilePath, sb.ToString());

            if (Options.Verbose)
                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - End");
        }

        (bool found, string file) Search(string[] files, string namespaceName, string className, string methodName)
        {
            var foundNamespace = false;
            var foundClass = false;
            var foundMethod = false;
            var fileName = "";

            foreach (var file in files)
            {
                if (Options.Verbose)
                    _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Search test with namespace:{namespaceName}, classname:{className}, methodname: {methodName} from source file -> {file}");

                var sourceLinesFile = System.IO.File.ReadAllLines(file);

                sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(namespaceName), (sourceLineFile, i) =>
                {
                    if (Options.Verbose)
                        _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Found Namespace {namespaceName} into file {file} at line {i}");

                    foundNamespace = true;
                    fileName = file;
                }, true);

                if (foundNamespace)
                {
                    sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(className), (sourceLineFile, i) =>
                    {
                        if (Options.Verbose)
                            _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Found Class {className} into file {file} at line {i}");

                        foundClass = true;
                    }, true);

                    if (foundClass)
                        sourceLinesFile.ForEach((sourceLineFile) => sourceLineFile.Contains(methodName), (sourceLineFile, i) =>
                        {
                            if (Options.Verbose)
                                _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Found Method {methodName} into file {file} at line {i}");

                            foundMethod = true;
                        }, true);
                    else
                    {
                        if (Options.Verbose)
                        {
                            _console.WriteLine($"{TestLogger.FriendlyName}:{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Not Found Class {className} into file {file}, Try next file");
                        }
                    }
                }

                if (foundNamespace && foundClass && foundMethod)
                    break;
            }

            return (foundNamespace && foundClass && foundMethod, fileName);
        }
    }

    public record TestResultInfo(string DisplayName, TestOutcome Outcome, double Duration, string FullyQualifiedName, string TestNamespace, string TestClassName, string TestMethodName);
}
