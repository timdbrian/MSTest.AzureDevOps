using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MSTest.AzureDevOps.Services.Models.WorkItems
{
	[XmlRoot("parameterSet")]
	public class ParameterSet
	{
		[XmlArray("paramNames")]
		[XmlArrayItem("param")]
		public List<string> ParameterNames
		{ get; set; }

		[XmlArray("paramData")]
		[XmlArrayItem("dataRow")]
		public List<ParameterDataRow> ParameterDataRows
		{ get; set; }
	}

	public class ParameterDataRow
	{
		[XmlAttribute("id")]
		public int Id
		{ get; set; }

		[XmlElement("kvp")]
		public List<Kvp> KeyValues
		{ get; set; }
	}

	public class Kvp
	{
		[XmlAttribute("value")]
		public string Value
		{ get; set; }

		[XmlAttribute("key")]
		public string Key
		{ get; set; }
	}

	//public class ParameterData
	//{
	//	[XmlAttribute("lastId")]
	//	public int LastId
	//	{ get; set; }

	//	[XmlElement("dataRow")]
	//	public List<ParameterDataRow> DataRows
	//	{ get; set; }

	//}
}
