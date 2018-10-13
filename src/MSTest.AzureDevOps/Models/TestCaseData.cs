using System.Collections.Generic;

namespace MSTest.AzureDevOps.Models
{
    /// <summary>
    /// Data from test case.
    /// </summary>
    public class TestCaseData
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestCaseData()
        {
            Parameters = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Test case/work Item id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Test case title.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Current iteration parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; internal set; }

        #endregion
    }
}
