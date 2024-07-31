using System;
using System.Collections.Generic;

namespace SonarqubeTestLogger;

public partial class TestLoggerOptions
{
    public string LogFilePath { get; set; } = "";
    public string PathTestProject { get; set; } = "";
    public string PathsExcluded { get; set; } = "bin,obj";
    public bool Verbose { get; set; } = false;
}

public partial class TestLoggerOptions
{
    public static TestLoggerOptions Resolve(IReadOnlyDictionary<string, string> parameters) =>
        new()
        {
            LogFilePath = parameters.GetValueOrDefault("LogFilePath") ?? "SonarQubeReportTest.xml",
            PathTestProject = parameters.GetValueOrDefault("PathTestProject") ?? Environment.CurrentDirectory,
            PathsExcluded = parameters.GetValueOrDefault("PathsExcluded") ?? "bin,obj",
            Verbose = Convert.ToBoolean(parameters.GetValueOrDefault("Verbose") ?? "false")
        };
}
