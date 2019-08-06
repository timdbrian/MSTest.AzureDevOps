using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSTest.AzureDevOps.Services.Models.WorkItems
{
	public partial class ParameterMap
	{
		[JsonProperty("parameterMap")]
		public List<ParameterMapItem> ParameterMappings { get; set; }

		[JsonProperty("sharedParameterDataSetIds")]
		public List<int> SharedParameterDataSetIds { get; set; }

		[JsonProperty("rowMappingType")]
		public int RowMappingType { get; set; }
	}

	public partial class ParameterMapItem
	{
		[JsonProperty("localParamName")]
		public string LocalParamName { get; set; }

		[JsonProperty("sharedParameterName")]
		public string SharedParameterName { get; set; }

		[JsonProperty("sharedParameterDataSetId")]
		public int SharedParameterDataSetId { get; set; }
	}
}
