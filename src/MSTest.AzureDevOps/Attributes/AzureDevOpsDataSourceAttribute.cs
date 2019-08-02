using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.AzureDevOps.Models;
using MSTest.AzureDevOps.Services;
using MSTest.AzureDevOps.Services.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MSTest.AzureDevOps.Attributes
{
	/// <summary>
	/// Azure DevOps data source for MSTest V2.
	/// </summary>
	public class AzureDevOpsDataSourceAttribute : Attribute, ITestDataSource
	{
		#region Properties

		/// <summary>
		/// Work item id for get data.
		/// </summary>
		public int WorkItemId { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="workItem">Work item id for get data</param>
		public AzureDevOpsDataSourceAttribute(int workItem)
		{
			this.WorkItemId = workItem;
		}

		#endregion

		#region ITestDataSource Implementation

		/// <summary>
		/// Gets the test data from custom test data source.
		/// </summary>
		/// <param name="methodInfo">The method info of test method.</param>
		/// <returns>Test data for calling test method.</returns>
		public IEnumerable<object[]> GetData(MethodInfo methodInfo)
		{
			var testData = new List<object[]>();                                        //Set up collection to hold all the test case data
			var workItemRequest = WorkItemService.Instance.GetById(this.WorkItemId);    //Get work item's information by id
			Task.WaitAll(workItemRequest);                                              //Await the HTTP response
			var workItem = workItemRequest.Result;                                      //Main work item after it is returned by the WorkItemService
			if (string.IsNullOrWhiteSpace(workItem.TestDataSource))                     //Check if service parameters data was retrieved
			{
				return null;
			}
			//TODO: Add exception handling

			var testParameters = Deserializers.GetTestParameters(workItem.Parameters);            //Convert XML parameters in work item to a usable collection

			//Test the data source to determine type (XML/JSON : Enclosed Data/Shared Data)
			if (workItem.TestDataSource.StartsWith('<') && workItem.TestDataSource.EndsWith('>'))
			{
				//Standard parameter data
				var parameterData = Deserializers.GetTestParameterValues(workItem.TestDataSource);

				foreach (Dictionary<string, string> dataRow in parameterData)       //Get each row item contained in the test case
				{
					if (testParameters.Parameters.Count != dataRow.Keys.Count)      //Ensure that the parameters defined in the test case, are all contained in the data row
					{
						Console.WriteLine($"Warning! Test parameters count is {testParameters.Parameters.Count} but the data row keys contains {dataRow.Keys.Count} keys (they are supposed to match). Please ensure the test cases and parameters have the correct links");
					}

					var testCaseData = new TestCaseData
					{
						Id = this.WorkItemId,
						Title = workItem.Title,
						Parameters = dataRow
					};

					testData.Add(new[] { testCaseData });                           //Add the items  to the final collection
				}
			}
			if (workItem.TestDataSource.StartsWith("{\"parameterMap"))              //Indicates a JSON result, with shared parameter data
			{
				//Parameter data is in a shared parameter item, need to read map
				var mapping = Deserializers.GetSharedParameterMapping(workItem.TestDataSource);

				if (testParameters.Parameters.Count != mapping.ParameterMappings.Count)
				{
					Console.WriteLine($"Warning! Number of parameters in test case id {this.WorkItemId}({testParameters.Parameters.Count} parameters) do not match the number of mapped parameters ({mapping.ParameterMappings.Count})");
				}
				//Azure DevOps can link local parameters to multiple shared test parameters, across multiple work item ids
				//Retrieve each shared parameter data set
				var parameterRequests = new Dictionary<int, Task<ParameterSet>>();
				foreach (var dataSetId in mapping.SharedParameterDataSetIds)        // Create all the requests for various datasets
				{
					parameterRequests.Add(dataSetId, WorkItemService.Instance.GetSharedParametersById(dataSetId));
				}
				Task.WaitAll(parameterRequests.Values.ToArray());                   // Await all the responses

				var dataSets = new Dictionary<int, ParameterSet>();
				foreach (var response in parameterRequests)
				{
					dataSets.Add(response.Key, response.Value.Result);              // Store the results in a dcitionary against the dataSetId for further reference
				}



				Dictionary<int, Dictionary<string, string>> parameterValues = new Dictionary<int, Dictionary<string, string>>();
				foreach (var dataSet in dataSets)                       //Ensure that the parameters defined in the test case, are all contained in the shared data mappings
				{
					//For every local parameter which is mapped to a shared parameter, get the value of the sharedParameter and store that.
					foreach (var dataRow in dataSet.Value.ParameterDataRows)
					{
						if (!parameterValues.ContainsKey(dataRow.Id))
						{
							parameterValues.Add(dataRow.Id, new Dictionary<string, string>());
						}
						var values = parameterValues[dataRow.Id];
						foreach (var kvp in dataRow.KeyValues)
						{
							string mappedKey = mapping.ParameterMappings.SingleOrDefault(pm => pm.SharedParameterDataSetId == dataSet.Key && pm.SharedParameterName == kvp.Key)?.LocalParamName;
							if (string.IsNullOrEmpty(mappedKey))
							{
								//Something went wrong. Log the mapping error, that a local parameter name could not be found for the shared parameter name. Perhaps the shared parameter name is not mapped locally?
								Console.WriteLine($"Error! Local parameter mapping was not found for a shared parameter key {mappedKey}. Perhaps the shared parameter name is not mapped locally?");
							}
							else
							{
								values.Add(mappedKey, kvp.Value);
							}
						}
					}
				}

				foreach (var parameterValue in parameterValues)
				{
					var testCaseData = new TestCaseData
					{
						Id = this.WorkItemId,
						Title = workItem.Title,
						Parameters = parameterValue.Value
					};

					testData.Add(new[] { testCaseData });
				}
			}
			return testData;
		}

		/// <summary>
		/// Gets the display name corresponding to test data row for displaying in TestResults.
		/// </summary>
		/// <param name="methodInfo">The method info of test method.</param>
		/// <param name="data">The test data which is passed to test method.</param>
		/// <returns>The System.String.</returns>
		public string GetDisplayName(MethodInfo methodInfo, object[] data)
		{
			if (data != null)
			{
				return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));
			}

			return null;
		}

		#endregion


	}
}
