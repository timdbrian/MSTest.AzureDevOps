namespace MSTest.AzureDevOps.Services.Models.WorkItems
{
	/// <summary>
	/// Service response model.
	/// </summary>
	public class GetByIdResponseModel
	{
		#region Properties

		public int Id { get; set; }
		public string Title { get; set; }
		public string TestDataSource { get; set; }
		public string Parameters { get; set; }

		#endregion
	}
}
