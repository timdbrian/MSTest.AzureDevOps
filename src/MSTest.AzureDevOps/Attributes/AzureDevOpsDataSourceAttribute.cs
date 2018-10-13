using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.AzureDevOps.Models;
using MSTest.AzureDevOps.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;

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
        private int _workItemId { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workItem">Work item id for get data</param>
        public AzureDevOpsDataSourceAttribute(int workItem)
        {
            _workItemId = workItem;
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
            // Get work item's information by id
            var workItemAsync = WorkItemService.Instance.GetById(_workItemId);
            workItemAsync.Wait();

            // Check if service parameters data was retrieved
            if (string.IsNullOrWhiteSpace(workItemAsync.Result.TestDataSource))
                return null;

            var arrayTestCaseData = new List<object[]>();

            // convert parameters data (from work item) to dataset
            var xmlParametersStream = new StringReader(workItemAsync.Result.TestDataSource);
            var dataSetParameters = new DataSet();
            dataSetParameters.ReadXml(xmlParametersStream);

            // Iterate each row from parameters
            foreach (DataRow row in dataSetParameters.Tables[0].Rows)
            {
                // Create test case object
                var testCaseData = new TestCaseData()
                {
                    Id = _workItemId,
                    Title = workItemAsync.Result.Title
                };

                foreach (DataColumn column in dataSetParameters.Tables[0].Columns)
                    testCaseData.Parameters.Add(column.ColumnName, row[column.ColumnName].ToString());

                arrayTestCaseData.Add(new[] { testCaseData });
            }

            // Return test data array
            return arrayTestCaseData.ToArray();
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
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));

            return null;
        }

        #endregion
    }
}
