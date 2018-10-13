using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MSTest.AzureDevOps.Configs
{
    /// <summary>
    /// MSTest configuration.
    /// </summary>
    internal sealed class MSTestConfig
    {
        private static MSTestConfig instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// Private constructor (for singleton)
        /// </summary>
        private MSTestConfig() { }

        /// <summary>
        /// Singleton implementation.
        /// </summary>
        internal static MSTestConfig Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");

                        // Check if settings file was created
                        if (!File.Exists(settingsPath))
                            throw new Exception($"Configuration file (settings.json) doesn't exists. Create it and confirm that the 'Copy to Output Directory' property is checked with 'Copy always' or 'Copy if newer'.");

                        // Set up of configuration builder
                        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("settings.json");

                        // Load configuration file
                        var config = builder.Build();

                        // Set configurations to instance
                        instance = new MSTestConfig()
                        {
                            AzureDevOps = new AzureDevOpsComposition()
                            {
                                AccountName = config["MSTest:AzureDevOps:AccountName"],
                                PAT = config["MSTest:AzureDevOps:PAT"]
                            }
                        };
                    }

                    return instance;
                }
            }
        }
         
        internal AzureDevOpsComposition AzureDevOps { get; set; }

        internal class AzureDevOpsComposition
        {
            public string AccountName { get; set; }
            public string PAT { get; set; }
        }
    }
}
