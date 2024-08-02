# Sonarqube Test Logger

Sonarqube xml report extension for [Visual Studio Test Platform](https://github.com/microsoft/vstest).

[![NuGet Downloads](https://img.shields.io/nuget/dt/Maurosoft.SonarqubeTestLogger)](https://www.nuget.org/packages/Maurosoft.SonarqubeTestLogger/)

## Packages

| Logger    | Stable Package |
| --------- | -------------- |
| Sonarqube | ![NuGet Version](https://img.shields.io/nuget/v/Maurosoft.SonarqubeTestLogger)|

## Usage

The Sonarqube Test Logger generates xml reports in the [Generic test execution report format](https://docs.sonarsource.com/sonarqube/latest/analyzing-source-code/test-coverage/generic-test-data/#generic-test-execution).

To use the logger, follow these steps:

1. Add a reference to the [Sonarqube Test Logger](https://www.nuget.org/packages/Maurosoft.SonarqubeTestLogger) nuget package in test project
   ```none
   dotnet add package Maurosoft.SonarqubeTestLogger
   ```
2. Use the following command line in tests
   ```none
   dotnet test --logger:sonarqube
   ```

3. Test results are generated in the `TestResults` directory  relative to the `test.csproj`. The default report name is `SonarqubeReportTest.xml`

A path for the report file can be specified as follows:

```none
dotnet test --logger:"sonarqube;LogFilePath=test-result.xml"
```

`test-result.xml` will be generated in the same directory as `test.csproj`.

**Note:** the arguments to `--logger` should be in quotes since `;` is treated as a command delimiter in shell.

## Logger Configuration

- **LogFileName**: Use LogFileName to specify the name of the output log file. The file will be created in the default results directory (TestResults) relative to test project.
    ```none
    # Assume we have this directory structure of tests
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj

    > dotnet test --logger:"sonarqube;LogFileName=mytestfile.xml"

    # Note the output file
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── TestResults
    │   │   ├── mytestfile.xml    # test result file
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj
    ```

- **LogFilePath**: Use this option to provide an absolute path for the result file. The parent directory will be created if it doesn't exist.
    ```none
    # Assume we have this directory structure of tests
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj

    > dotnet test --logger:"sonarqube;LogFilePath=mytestfile.xml"

    # Note the output file
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── mytestfile.xml    # test result file
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj
    ```

- **Verbose**: If true, log message to console
    ```none
    # Assume we have this directory structure of tests
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj

    > dotnet test --logger:"sonarqube;Verbose=true"
    
    The output on console:

    sonarqube - Start
    sonarqube - LogFileName      ->
    sonarqube - LogFilePath      -> mytestfile.xml
    sonarqube - TestRunDirectory -> C:\Users\Test\source\repos\TestProject1\TestProject1\TestResults
    sonarqube - PathSourcesTest: -> C:\Users\Test\source\repos\TestProject1\TestProject1
    sonarqube - Found Test with Namespace -> TestProject1, ClassName -> Class1, MethodName -> TestMethod1 from TestResults
    sonarqube - Found Test with Namespace -> TestProject1, ClassName -> Class1, MethodName -> TestMethod2 from TestResults
    sonarqube - Search test with namespace:TestProject1, classname:Class1, methodname: Test1 from source file -> C:\Users\Test\source\repos\TestProject1\TestProject1\Class1.cs
    sonarqube - Found Namespace TestProject1 into file C:\Users\Test\source\repos\TestProject1\TestProject1\Class1.cs at line 7
    sonarqube - Found Class Class1 into file C:\Users\Test\source\repos\TestProject1\TestProject1\Class1.cs at line 10
    sonarqube - Found Method TestMethod1 into file C:\Users\Test\source\repos\TestProject1\TestProject2\Class1.cs at line 13
    ...
    ...
    sonarqube - End

    # Note the output file
    > tree
    .
    │ TestProject1.sln
    ├── TestProject1
    │   ├── TestResults
    │   │   ├── mytestfile.xml    # test result file
    │   ├── UnitTest1.cs
    │   └── TestProject1.csproj
    ```

## License

MIT
