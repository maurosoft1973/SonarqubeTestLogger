﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace SonarqubeTestLogger.Test.Common;

internal class FakeTestLoggerEvents : TestLoggerEvents
{
    public override event EventHandler<TestRunMessageEventArgs>? TestRunMessage;
    public override event EventHandler<TestRunStartEventArgs>? TestRunStart;
    public override event EventHandler<TestResultEventArgs>? TestResult;
    public override event EventHandler<TestRunCompleteEventArgs>? TestRunComplete;
    public override event EventHandler<DiscoveryStartEventArgs>? DiscoveryStart;
    public override event EventHandler<TestRunMessageEventArgs>? DiscoveryMessage;
    public override event EventHandler<DiscoveredTestsEventArgs>? DiscoveredTests;
    public override event EventHandler<DiscoveryCompleteEventArgs>? DiscoveryComplete;
}
