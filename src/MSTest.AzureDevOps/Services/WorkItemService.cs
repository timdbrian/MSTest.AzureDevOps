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
		private static readonly Lazy<WorkItemService> instance = new Lazy<WorkItemService>(new WorkItemService());

		/// <summary>
		/// Private constructor. Should only be insantiated via the Lazy Load constructor so as to ensure a singleton
		/// </summary>
		private WorkItemService() { }

		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static WorkItemService Instance
		{
			get
			{
				return instance.Value;
			}
		}

		/// <summary>
		/// Get work item by work item id.
		/// </summary>
		/// <param name="workItemId">Work item id</param>
		public async Task<GetByIdResponseModel> GetById(int workItemId)
		{
			using (var httpClient = new HttpClient()) // Initialize http client
			{
				// Configure headers (including authorization)
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				if (MSTestConfig.Instance.AzureDevOps.TokenType == MSTestConfig.TokenType.Bearer)
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MSTestConfig.Instance.AzureDevOps.Token);
				}
				if (MSTestConfig.Instance.AzureDevOps.TokenType == MSTestConfig.TokenType.Basic)
				{
					string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format(":{0}", MSTestConfig.Instance.AzureDevOps.Token)));
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
				}
				var collectionUri = new Uri(MSTestConfig.Instance.AzureDevOps.CollectionUri);
				var workItemUri = new Uri(collectionUri, $"{MSTestConfig.Instance.AzureDevOps.ProjectIdentifier}/_apis/wit/workitems/{workItemId}?api-version={MSTestConfig.Instance.AzureDevOps.ApiVersion}");

				var response = await httpClient.GetAsync(workItemUri); // Retrieve work item information
				if (!response.IsSuccessStatusCode) // Check if was sucessfull
				{
					throw new Exception($"Error getting work item {workItemId} in {workItemUri}: {await response.Content.ReadAsStringAsync()}");
				}
				dynamic jObject = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()); // Deserialize json to typed class
				return new GetByIdResponseModel // Return information
				{
					Id = jObject.id,
					Title = jObject.fields["System.Title"],
					TestDataSource = jObject.fields["Microsoft.VSTS.TCM.LocalDataSource"],
					Parameters = jObject.fields["Microsoft.VSTS.TCM.Parameters"]
				};
			}
		}

		public async Task<ParameterSet> GetSharedParametersById(int id)
		{
			var sharedParameters = await this.GetById(id);
			return Deserializers.GetSharedParameterSet(sharedParameters.Parameters);

		}
	}
}