using MSTest.AzureDevOps.Configs;
using MSTest.AzureDevOps.Services.Models.WorkItems;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.AzureDevOps.Services
{
    /// <summary>
    /// Work item service.
    /// </summary>
    internal class WorkItemService
    {
        #region Properties

        private static WorkItemService instance = null;
        private static readonly object padlock = new object();

        #endregion

        #region Singleton Pattern

        /// <summary>
        /// Private constructor.
        /// </summary>
        private WorkItemService() { }

        /// <summary>
        /// Class instance.
        /// </summary>
        public static WorkItemService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new WorkItemService();

                    return instance;
                }
            }
        }

        #endregion

        #region Routes

        #region GET

        /// <summary>
        /// Get work item by work item id.
        /// </summary>
        /// <param name="workItemId">Work item id</param>
        public async Task<GetByIdResponseModel> GetById(int workItemId)
        {
            // Initialize http client
            using (var httpClient = new HttpClient())
            {
                // Configure headers (including authorization)
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format(":{0}", MSTestConfig.Instance.AzureDevOps.PAT))));

                // Retrieve work item information
                var response = await httpClient
                    .GetAsync($"https://dev.azure.com/{MSTestConfig.Instance.AzureDevOps.AccountName}/_apis/wit/workitems/{workItemId}?api-version=5.0-preview.3&fields=System.Title,Microsoft.VSTS.TCM.LocalDataSource");

                // Check if was sucessfull
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error getting work item {workItemId} in {MSTestConfig.Instance.AzureDevOps.AccountName} account: {response.Content.ReadAsStringAsync().Result}");

                // Deserialize json to typed class
                dynamic jObject = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                // Return information
                return new GetByIdResponseModel()
                {
                    Id = jObject.id,
                    Title = jObject.fields["System.Title"],
                    TestDataSource = jObject.fields["Microsoft.VSTS.TCM.LocalDataSource"] != null ? jObject.fields["Microsoft.VSTS.TCM.LocalDataSource"] : null
                };
            }
        }

        #endregion

        #endregion
    }
}