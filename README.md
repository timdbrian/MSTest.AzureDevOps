MSTest.AzureDevOps
==================

Data driven unit tests/automated tests with Azure DevOps test case datasource in MSTest v2.

Build Status
------------

[![Build Status](https://dev.azure.com/l3oferreira/GitHub/_apis/build/status/GitHub-ASP.NET%20Core-CI)](https://dev.azure.com/l3oferreira/GitHub/_build/latest?definitionId=22)


Instructions
------------

Installing Nuget Package
------------------------

 Package | NuGet Stable | 
| ------- | ------------ |
| [l3oferreira.MSTest.AzureDevOps](https://www.nuget.org/packages/l3oferreira.MSTest.AzureDevOps/) | ![Nuget](https://img.shields.io/nuget/v/l3oferreira.MSTest.AzureDevOps.svg)



Creating and configuring settings.json
--------------------------------------
After installing the Nuget package, the first step is to create a file named settings.json and confirm that the **Copy to Output Directory** property is checked with **Copy always** or **Copy if newer**.

![Visual Studio - settings.json Properties](https://github.com/l3oferreira/MSTest.AzureDevOps/blob/master/images/visual-studio-settings-config.png?raw=true)

File content:

```json
{
  "MSTest": {
    "AzureDevOps": {
      "PAT": "<<Personal Access Token>>",
      "AccountName": "<<Account Name>>"
    }
  }
}
```

*To get PAT (Personal Access Token), follow these instructions: [Authenticate access with personal access tokens for Azure DevOps Services and TFS](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts)*


Creating test case in Azure DevOps
----------------------------------
When creating the test case, you must be informed the parameters, to be used in the automated tests. For each line of parameters *(each iteration)*, there will be a new execution of the test *(in the example below, the same method will be executed twice)*.

![Azure DevOps - Test Case](https://github.com/l3oferreira/MSTest.AzureDevOps/blob/master/images/azure-devops-test-case.png?raw=true)


Implementing test method
------------------------
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

![Visual Studio - Debugging](https://github.com/l3oferreira/MSTest.AzureDevOps/blob/master/images/visual-studio-debugging.png?raw=true)
