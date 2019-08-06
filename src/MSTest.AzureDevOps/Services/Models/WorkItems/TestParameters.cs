using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MSTest.AzureDevOps.Services.Models.WorkItems
{
	[XmlRoot("parameters")]
	public class TestParameterList
	{
		[XmlElement("param")]
		public List<TestParameter> Parameters
		{ get; set; }
	}

	public class TestParameter
	{
		[XmlAttribute("name")]
		public string Name
		{ get; set; }

		[XmlAttribute("bind")]
		public string Bind
		{ get; set; }
	}
}
