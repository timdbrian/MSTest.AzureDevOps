# MSTest.AzureDevOps

## Build Status
[![Build Status](https://dev.azure.com/l3oferreira/GitHub/_apis/build/status/GitHub-ASP.NET%20Core-CI)](https://dev.azure.com/l3oferreira/GitHub/_build/latest?definitionId=22)


## Nuget Package
![NuGet](https://img.shields.io/nuget/v/l3oferreira.MSTest.AzureDevOps.svg)


## Instructions

### Creating test case in Azure DevOps

When creating the test case, you must be informed the parameters, to be used in the automated tests. For each line of parameters *(each iteration)*, there will be a new execution of the test *(in the example below, the same method will be executed twice)*.

![Azure DevOps Test Case](https://github.com/l3oferreira/MSTest.AzureDevOps/blob/master/images/azure-devops-test-case.png?raw=true)

### Implementing test method

When implementing the test method, you must include the **AzureDevOpsDataSource** attribute, and pass as the parameter, the id of the test case work item. The method you create must have only one argument, of type **TestCaseData**. This parameter contains the basic information of the test case *(id, title and parameters)*.

Example usage:

```csharp
[AzureDevOpsDataSource(532)]
[TestMethod]
public void UnitTestMethod(TestCaseData testCaseData)
{
    TestContext.WriteLine($"Initializing test case {testCaseData.Id} - {testCaseData.Title}");

    WebDriver.Navigate().GoToUrl(TestContext.Properties["url"].ToString());

    WebDriver.FindElementById("username").SendKeys(testCaseData.Parameters["username"]);
    WebDriver.FindElementById("password").SendKeys(testCaseData.Parameters["password"] + Keys.Enter);
}
```

Example debugging:

![Visual Studio Debugging](https://github.com/l3oferreira/MSTest.AzureDevOps/blob/master/images/visual-studio-debugging.png?raw=true)
