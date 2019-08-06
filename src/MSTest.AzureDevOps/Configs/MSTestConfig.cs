using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace MSTest.AzureDevOps.Configs
{
	/// <summary>
	/// MSTest configuration.
	/// </summary>
	internal sealed class MSTestConfig
	{
		private static readonly Lazy<MSTestConfig> instance = new Lazy<MSTestConfig>(LoadConfiguration);

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
				return instance.Value;
			}
		}

		private static MSTestConfig LoadConfiguration()
		{
			//Default values and environment values if available
			var accessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN");
			var tokenType = TokenType.Bearer;
			var apiVersion = "5.0";
			var collectionUri = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI");
			var project = Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECTID");
			var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
			if (File.Exists(settingsPath)) //If a settings file exists, use it below. 
			{
				// Set up of configuration builder
				var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("settings.json");
				var config = builder.Build(); // Load configuration files

				//Environment variables should override the settings file for when the tests are run by an Azure DevOps build agent
				if (string.IsNullOrEmpty(accessToken)) //If there's no environment access bearer token, use the settings file supplied PAT Basic token.
				{
					accessToken = config["MSTest:AzureDevOps:Token"];
					tokenType = TokenType.Basic;
				}

				if (!string.IsNullOrEmpty(config["MSTest:AzureDevOps:ApiVersion"])) //Override default version with settings if it exists
				{
					apiVersion = config["MSTest:AzureDevOps:ApiVersion"];
				}

				if (string.IsNullOrEmpty(collectionUri))
				{
					collectionUri = config["MSTest:AzureDevOps:CollectionUri"];
				}

				if (string.IsNullOrEmpty(project))
				{
					project = config["MSTest:AzureDevOps:ProjectIdentifier"];
				}
			}

			if (string.IsNullOrEmpty(accessToken))
			{
				throw new ArgumentNullException($"No access token was available through either environment variable SYSTEM_ACCESSTOKEN or through {settingsPath} file at MSTest:AzureDevOps:Token. Please make sure an access token is available as it's required to connect to AzDevOps");
			}

			if (string.IsNullOrEmpty(collectionUri))
			{
				throw new ArgumentNullException($"No collection URI was avaible through either the environment variable SYSTEM_TEAMFOUNDATIONCOLLECTIONURI or through the {settingsPath} file at MSTest:AzureDevOps:CollectionUri. Please make sure an Azure DevOps collection URI is available");
			}

			if (string.IsNullOrEmpty(project))
			{
				throw new ArgumentNullException($"No project name or identifier/GUID was available either through the environment variable SYSTEM_TEAMPROJECTID or through the {settingsPath} file at MSTest:AzureDevOps:ProjectIdentifier. Please make sure an Azure DevOps project name/guid is available");
			}

			// Set configurations to instance
			return new MSTestConfig
			{
				
				AzureDevOps = new AzureDevOpsComposition
				{
					Token = accessToken,
					TokenType = tokenType,
					ApiVersion = apiVersion,
					CollectionUri = collectionUri,
					ProjectIdentifier = project
				}
			};
		}

		internal AzureDevOpsComposition AzureDevOps { get; set; }

		internal class AzureDevOpsComposition
		{
			public string Token { get; set; }

			public string ApiVersion { get; set; }

			public string CollectionUri { get; set; }

			public string ProjectIdentifier { get; set; }

			public TokenType TokenType { get; set; }

		}

		internal enum TokenType
		{
			Bearer,
			Basic
		}
	}
}
