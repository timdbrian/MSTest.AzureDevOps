using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using MSTest.AzureDevOps.Configs;
using MSTest.AzureDevOps.Services.Models.WorkItems;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.AzureDevOps.Services
{
	/// <summary>
	/// Work item service.
	/// </summary>
	public class WorkItemService
	{
		public AzureDevOpsComposition Configuration { get; }

		public WorkItemService(MethodInfo methodInfo)
		{
			this.Configuration = new AzureDevOpsComposition(methodInfo?.DeclaringType?.Assembly);
		}

		/// <summary>
		/// Get work item by work item id.
		/// </summary>
		/// <param name="workItemId">Work item id</param>
		public async Task<GetByIdResponseModel> GetById(int workItemId)
		{
			try
			{
				using (var connection = new VssConnection(this.Configuration.CollectionUri, this.Configuration.Credentials))
				{
					using (var witClient = connection.GetClient<WorkItemTrackingHttpClient>())
					{
						var item = await witClient.GetWorkItemAsync(workItemId);
						return new GetByIdResponseModel
						{
							Id = item.Id ?? workItemId,
							Parameters = item.Fields["Microsoft.VSTS.TCM.Parameters"]?.ToString(),
							TestDataSource = item.Fields.TryGetValue("Microsoft.VSTS.TCM.LocalDataSource", out string val) ? val : null,
							Title = item.Fields["System.Title"]?.ToString()
						};
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error getting work item {workItemId} from {this.Configuration.CollectionUri}. {ex.Message}", ex);
			}
		}

		public async Task<ParameterSet> GetSharedParametersById(int id)
		{
			var sharedParameters = await this.GetById(id);
			return Deserializers.GetSharedParameterSet(sharedParameters.Parameters);

		}
	}
}