using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace SonarqubeTestLogger;

public partial class TestLoggerOptions
{
    public string LogFileName { get; set; } = "";
    public string LogFilePath { get; set; } = "";
    public string PathTestProject { get; set; } = "";
    public string PathsExcluded { get; set; } = "bin,obj";
    public string TestRunDirectory { get; set; } = "";
    public bool Verbose { get; set; } = false;
}

public partial class TestLoggerOptions
{
    public static TestLoggerOptions Resolve(IReadOnlyDictionary<string, string?> parameters) =>
        new()
        {
            LogFileName = parameters.GetValueOrDefault("LogFileName") ?? "",
            LogFilePath = parameters.GetValueOrDefault("LogFilePath") ?? "",
            TestRunDirectory = parameters.GetValueOrDefault(DefaultLoggerParameterNames.TestRunDirectory) ?? Environment.CurrentDirectory,
            PathTestProject = parameters.GetValueOrDefault("PathTestProject") ?? Environment.CurrentDirectory,
            PathsExcluded = parameters.GetValueOrDefault("PathsExcluded") ?? "bin,obj",
            Verbose = Convert.ToBoolean(parameters.GetValueOrDefault("Verbose", "false") ?? "false")
        };
}
