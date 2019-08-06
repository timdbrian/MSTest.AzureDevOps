using MSTest.AzureDevOps.Services.Models.WorkItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MSTest.AzureDevOps.Services
{
	public static class Deserializers
	{
		public static TestParameterList GetTestParameters(string xml)
		{
			using (StringReader reader = new StringReader(xml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TestParameterList));
				return (TestParameterList)serializer.Deserialize(reader);
			}
		}

		public static List<Dictionary<string, string>> GetTestParameterValues(string xml) //First list is rows, internal dictionary is key/parameter and value
		{
			var items = new List<Dictionary<string, string>>();
			using (var xmlParametersStream = new StringReader(xml))
			{
				var dataSetParameters = new DataSet();
				dataSetParameters.ReadXml(xmlParametersStream, XmlReadMode.ReadSchema);

				foreach (DataRow row in dataSetParameters.Tables[0].Rows)
				{
					var item = new Dictionary<string, string>(); //Key/Value
					foreach (DataColumn column in dataSetParameters.Tables[0].Columns)
					{
						item.Add(column.ColumnName, Convert.ToString(row[column.ColumnName])); //Only strings as values
					}
					items.Add(item);
				}
			}
			return items;
		}

		public static ParameterMap GetSharedParameterMapping(string json)
		{
			return JsonConvert.DeserializeObject<ParameterMap>(json);
		}

		public static ParameterSet GetSharedParameterSet(string xml)
		{
			using (StringReader reader = new StringReader(xml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ParameterSet));
				return (ParameterSet)serializer.Deserialize(reader);
			}
		}
	}
}
