# Sonarqube Test Logger

Sonarqube xml report extension for [Visual Studio Test Platform](https://github.com/microsoft/vstest).

[![NuGet Downloads](https://img.shields.io/nuget/dt/Maurosoft.SonarqubeTestLogger)](https://www.nuget.org/packages/Maurosoft.SonarqubeTestLogger/)

## Packages

| Logger    | Stable Package |
| --------- | -------------- |
| SonarQube | [![NuGet](https://img.shields.io/nuget/v/Maurosoft.SonarqubeTestLogger.svg)](https://www.nuget.org/packages/Maurosoft.SonarqubeTestLogger/)|

## Usage

The Sonarqube Test Logger generates xml reports in the [Generic test execution report format](https://docs.sonarsource.com/sonarqube/latest/analyzing-source-code/test-coverage/generic-test-data/#generic-test-execution).

To use the logger, follow these steps:

1. Add a reference to the [Sonarqube Test Logger](https://www.nuget.org/packages/Maurosoft.SonarqubeTestLogger) nuget package in test project
   ```none
   ```
1. 2. Use the following command line in tests
   ```none
   dotnet test --logger:sonarqube
   ```

3. Test results are generated in the `TestResults` directory relative to the `test.csproj`

A path for the report file can be specified as follows:

```none
dotnet test --logger:"sonarqube;LogFilePath=test-result.xml"
```

`test-result.xml` will be generated in the same directory as `test.csproj`.

**Note:** the arguments to `--logger` should be in quotes since `;` is treated as a command delimiter in shell.

## License

MIT