namespace SonarqubeTestLogger.Test.UnitTest;

[TestClass]
public class ExtensionMethodTest
{
    [TestMethod]
    [DataRow("SonarQubeTestLogger.Test.UnitTest1.TestMethod1", "SonarQubeTestLogger.Test")]
    [DataRow("SonarQubeTestLogger.UnitTest1.TestMethod1", "SonarQubeTestLogger")]
    [DataRow("", "")]
    public void ExtensionMethodTest_ToExtractNamespaceFromFQDN_Should_Return_Namespace_When_The_FullyQualifiedName_Its_Valid(
        string fullyQualifiedName,
        string namespaceExpected
    )
    {
        //Act
        var namespaceActual = fullyQualifiedName.ToExtractNamespaceFromFQDN();

        //Assert
        Assert.AreEqual(namespaceExpected, namespaceActual);
    }

    [TestMethod]
    [DataRow("SonarQubeTestLogger.Test.UnitTest.UnitTest1.TestMethod1", "UnitTest1")]
    [DataRow("SonarQubeTestLogger.Test.UnitTest1.TestMethod1", "UnitTest1")]
    [DataRow("SonarQubeTestLogger.UnitTest1.TestMethod1", "UnitTest1")]
    [DataRow("SonarQubeTestLogger", "")]
    [DataRow("", "")]
    public void ExtensionMethodTest_ToExtractClassFromFQDN_Should_Return_ClassName_When_The_FullyQualifiedName_Its_Valid(
        string fullyQualifiedName,
        string classNameExpected
    )
    {
        //Act
        var classNameActual = fullyQualifiedName.ToExtractClassFromFQDN();

        //Assert
        Assert.AreEqual(classNameExpected, classNameActual);
    }

    [TestMethod]
    [DataRow("SonarQubeTestLogger.Test.UnitTest.UnitTest1.TestMethod1", "TestMethod1")]
    [DataRow("SonarQubeTestLogger.Test.UnitTest1.TestMethod1", "TestMethod1")]
    [DataRow("SonarQubeTestLogger.UnitTest1.TestMethod1", "TestMethod1")]
    [DataRow("SonarQubeTestLogger", "")]
    [DataRow("", "")]
    public void ExtensionMethodTest_ToExtractMethodFromFQDN_Should_Return_MethodName_When_The_FullyQualifiedName_Its_Valid(
        string fullyQualifiedName,
        string methodNameExpected
    )
    {
        //Act
        var fqdnActual = fullyQualifiedName.ToExtractMethodFromFQDN();

        //Assert
        Assert.AreEqual(methodNameExpected, fqdnActual);
    }
}
