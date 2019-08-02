using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.AzureDevOps.Attributes;
using MSTest.AzureDevOps.Services;

namespace MSTestAzureDevOps.Tests
{
	[TestClass]
	public class DeserializerTests
	{

		[TestMethod]
		public void CanRead_ParameterList()
		{
			string parameterXml = "<parameters><param name=\"Parameter1\" bind=\"default\"/><param name=\"Parameter2\" bind=\"default\"/><param name=\"Parameter3\" bind=\"default\"/><param name=\"Parameter4\" bind=\"default\"/></parameters>";
			var testParams = Deserializers.GetTestParameters(parameterXml);

			Assert.IsNotNull(testParams, $"Parameters XML returned a null object for {parameterXml}");

			Assert.IsTrue(testParams.Parameters.Count == 4, $"Not all 4 paramerters were deserialized for {parameterXml}");

			Assert.IsTrue(testParams.Parameters[0].Name == "Parameter1");
			Assert.IsTrue(testParams.Parameters[1].Name == "Parameter2");
			Assert.IsTrue(testParams.Parameters[2].Name == "Parameter3");
			Assert.IsTrue(testParams.Parameters[3].Name == "Parameter4");
		}

		[TestMethod]
		public void CanRead_StandardParameterValues()
		{
			string parameterDataXml = @"<NewDataSet>
										<xs:schema id='NewDataSet' 
											xmlns:xs='http://www.w3.org/2001/XMLSchema' 
											xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
											<xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:Locale=''>
												<xs:complexType>
													<xs:choice minOccurs='0' maxOccurs = 'unbounded'>
														<xs:element name='Table1'>
															<xs:complexType>
																<xs:sequence>
																	<xs:element name='Parameter1' type='xs:string' minOccurs='0' />
																	<xs:element name='Parameter2' type='xs:string' minOccurs='0' />
																	<xs:element name='Parameter3' type='xs:string' minOccurs='0' />
																	<xs:element name='Parameter4' type='xs:string' minOccurs='0' />
																	<xs:element name='Parameter5' type='xs:string' minOccurs='0' />
																</xs:sequence>
															</xs:complexType>
														</xs:element>
													</xs:choice>
												</xs:complexType>
											</xs:element>
										</xs:schema>
										<Table1>
											<Parameter1>testing1.1</Parameter1>
											<Parameter2>testing1.2</Parameter2>
											<Parameter3>testing1.3</Parameter3>
											<Parameter4>testing1.4</Parameter4>
											<Parameter5>testing1.5</Parameter5>
										</Table1>
										<Table1>
											<Parameter1>testing2.1</Parameter1>
											<Parameter2>testing2.2</Parameter2>
											<Parameter3>testing2.3</Parameter3>
											<Parameter4>testing2.4</Parameter4>
											<Parameter5>testing2.5</Parameter5>
										</Table1>
									</NewDataSet>";

			var testValues = Deserializers.GetTestParameterValues(parameterDataXml);

			Assert.IsNotNull(testValues);

			Assert.IsTrue(testValues.Count == 2);

			Assert.IsTrue(testValues[0]["Parameter1"] == "testing1.1");
			Assert.IsTrue(testValues[0]["Parameter2"] == "testing1.2");
			Assert.IsTrue(testValues[0]["Parameter3"] == "testing1.3");
			Assert.IsTrue(testValues[0]["Parameter4"] == "testing1.4");
			Assert.IsTrue(testValues[0]["Parameter5"] == "testing1.5");

			Assert.IsTrue(testValues[1]["Parameter1"] == "testing2.1");
			Assert.IsTrue(testValues[1]["Parameter2"] == "testing2.2");
			Assert.IsTrue(testValues[1]["Parameter3"] == "testing2.3");
			Assert.IsTrue(testValues[1]["Parameter4"] == "testing2.4");
			Assert.IsTrue(testValues[1]["Parameter5"] == "testing2.5");
		}

		[TestMethod]
		public void CanRead_SharedParameterMapping()
		{
			string mappingJson = "{\"parameterMap\": [{\"localParamName\": \"Parameter1\",\"sharedParameterName\": \"Parameter1\",\"sharedParameterDataSetId\": 111},{\"localParamName\": \"Parameter2\",\"sharedParameterName\": \"Parameter2\",\"sharedParameterDataSetId\": 111},{\"localParamName\": \"Parameter3\",\"sharedParameterName\": \"Parameter3\",\"sharedParameterDataSetId\": 111},{\"localParamName\": \"Parameter4\",\"sharedParameterName\": \"Parameter4\",\"sharedParameterDataSetId\": 111}],\"sharedParameterDataSetIds\": [111],\"rowMappingType\": 0}";

			var parameterMapping = Deserializers.GetSharedParameterMapping(mappingJson);

			Assert.IsNotNull(parameterMapping);

			Assert.IsTrue(parameterMapping.RowMappingType == 0);

			Assert.IsTrue(parameterMapping.SharedParameterDataSetIds.Count == 1 && parameterMapping.SharedParameterDataSetIds[0] == 111);

			Assert.IsTrue(parameterMapping.ParameterMappings.Count == 4);

			Assert.IsTrue(parameterMapping.ParameterMappings[0].LocalParamName == "Parameter1");
			Assert.IsTrue(parameterMapping.ParameterMappings[0].SharedParameterName == "Parameter1");
			Assert.IsTrue(parameterMapping.ParameterMappings[0].SharedParameterDataSetId == 111);

			Assert.IsTrue(parameterMapping.ParameterMappings[1].LocalParamName == "Parameter2");
			Assert.IsTrue(parameterMapping.ParameterMappings[1].SharedParameterName == "Parameter2");
			Assert.IsTrue(parameterMapping.ParameterMappings[1].SharedParameterDataSetId == 111);

			Assert.IsTrue(parameterMapping.ParameterMappings[2].LocalParamName == "Parameter3");
			Assert.IsTrue(parameterMapping.ParameterMappings[2].SharedParameterName == "Parameter3");
			Assert.IsTrue(parameterMapping.ParameterMappings[2].SharedParameterDataSetId == 111);

			Assert.IsTrue(parameterMapping.ParameterMappings[3].LocalParamName == "Parameter4");
			Assert.IsTrue(parameterMapping.ParameterMappings[3].SharedParameterName == "Parameter4");
			Assert.IsTrue(parameterMapping.ParameterMappings[3].SharedParameterDataSetId == 111);

		}

		[TestMethod]
		public void CanRead_SharedParameterData()
		{
			string parameterXml = "<parameterSet><paramNames><param>Parameter1</param><param>Parameter2</param><param>Parameter3</param></paramNames><paramData lastId=\"2\"><dataRow id=\"1\"><kvp value=\"testing1.1\" key=\"Parameter1\"/><kvp value=\"testing1.2\" key=\"Parameter2\"/><kvp value=\"testing1.3\" key=\"Parameter3\"/></dataRow><dataRow id=\"2\"><kvp value=\"testing2.1\" key=\"Parameter1\"/><kvp value=\"testing2.2\" key=\"Parameter2\"/><kvp value=\"testing2.3\" key=\"Parameter3\"/></dataRow></paramData></parameterSet>";
			var sharedParameters = Deserializers.GetSharedParameterSet(parameterXml);
			
			Assert.IsNotNull(sharedParameters, $"Parameters XML returned a null object for {parameterXml}");

			Assert.IsTrue(sharedParameters.ParameterNames.Count == 3);
			Assert.IsTrue(sharedParameters.ParameterNames[0] == "Parameter1");
			Assert.IsTrue(sharedParameters.ParameterNames[1] == "Parameter2");
			Assert.IsTrue(sharedParameters.ParameterNames[2] == "Parameter3");

			Assert.IsTrue(sharedParameters.ParameterDataRows.Count == 2);

			Assert.IsTrue(sharedParameters.ParameterDataRows[0].Id == 1);
			Assert.IsTrue(sharedParameters.ParameterDataRows[0].KeyValues.Count == 3);

			var kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[0].KeyValues[0].Key, sharedParameters.ParameterDataRows[0].KeyValues[0].Value);
			Assert.IsTrue(kvp.Key == "Parameter1" && kvp.Value == "testing1.1");
			kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[0].KeyValues[1].Key, sharedParameters.ParameterDataRows[0].KeyValues[1].Value);
			Assert.IsTrue(kvp.Key == "Parameter2" && kvp.Value == "testing1.2");
			kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[0].KeyValues[2].Key, sharedParameters.ParameterDataRows[0].KeyValues[2].Value);
			Assert.IsTrue(kvp.Key == "Parameter3" && kvp.Value == "testing1.3");

			Assert.IsTrue(sharedParameters.ParameterDataRows[1].Id == 2);
			Assert.IsTrue(sharedParameters.ParameterDataRows[0].KeyValues.Count == 3);

			kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[1].KeyValues[0].Key, sharedParameters.ParameterDataRows[1].KeyValues[0].Value);
			Assert.IsTrue(kvp.Key == "Parameter1" && kvp.Value == "testing2.1");
			kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[1].KeyValues[1].Key, sharedParameters.ParameterDataRows[1].KeyValues[1].Value);
			Assert.IsTrue(kvp.Key == "Parameter2" && kvp.Value == "testing2.2");
			kvp = System.Collections.Generic.KeyValuePair.Create(sharedParameters.ParameterDataRows[1].KeyValues[2].Key, sharedParameters.ParameterDataRows[1].KeyValues[2].Value);
			Assert.IsTrue(kvp.Key == "Parameter3" && kvp.Value == "testing2.3");
		}
	}
}
