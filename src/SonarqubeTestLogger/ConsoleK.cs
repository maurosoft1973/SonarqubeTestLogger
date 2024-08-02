using System;
using System.Collections.Generic;
using System.Text;

namespace SonarqubeTestLogger;
public class ConsoleK : IConsoleK
{
    public void WriteLine(string message) => Console.WriteLine(message);
}
