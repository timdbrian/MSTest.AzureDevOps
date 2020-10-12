using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.OAuth;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace MSTest.AzureDevOps.Configs
{
	public class AzureDevOpsComposition
	{
		public VssCredentials Credentials { get; }

		//public string ApiVersion { get;}

		public Uri CollectionUri { get; }

		public string ProjectIdentifier { get; }

		//public TokenType TokenType { get; }

		public AzureDevOpsComposition(Assembly assembly = null, string settingsFileName = "settings.json")
		{	
			if (assembly == null)
			{
				assembly = Assembly.GetEntryAssembly();
			}
			var settingsFile = new FileInfo(settingsFileName);
			if (!settingsFile.Exists && assembly != null)
			{
				settingsFile = new FileInfo(Path.Combine(assembly.Location, settingsFileName));
			}
			if (!settingsFile.Exists)
			{
				settingsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFileName));
			}

			var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
			if (settingsFile.Exists)
			{
				_ = builder.AddJsonFile(settingsFile.FullName, true);
			}
			else
			{
				Console.WriteLine($"Warning: No settings file was used as none was found at {settingsFile.FullName}");
			}
			if (assembly != null)
			{
				_ = builder.AddUserSecrets(assembly, true);
			}
			var configuration = builder.Build(); // Load configuration files

			string accessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN"); //First preference is the bearer token which is typically added during agent execution (agent)
			if (!string.IsNullOrEmpty(accessToken))
			{
				this.Credentials = new VssOAuthAccessTokenCredential(accessToken);
			}
			else if (!string.IsNullOrEmpty(configuration["MSTest:AzureDevOps:Token"])) //Second preference is to use any Token stored using configuration or secret manager (dev token)
			{
				this.Credentials = new VssBasicCredential(string.Empty, configuration["MSTest:AzureDevOps:Token"]);
			}
			else
			{
				//Last preference is to try and retrieve the credentials interactively for an AAD backed AzDevOps instance
				this.Credentials = new VssClientCredentials(false)
				{
					PromptType = CredentialPromptType.PromptIfNeeded
				}; 
				//this.Credentials.Storage = new VssClientCredentialStorage();
			}

			try
			{
				this.CollectionUri = new Uri(Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI") ?? configuration["MSTest:AzureDevOps:CollectionUri"]);
			}
			catch (Exception ex)
			{
				throw new ArgumentNullException($"No valid collection URI was avaible through either the environment variable SYSTEM_TEAMFOUNDATIONCOLLECTIONURI ({Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI") ?? "null"}) or through the {settingsFile.FullName} file at MSTest:AzureDevOps:CollectionUri({configuration["MSTest:AzureDevOps:CollectionUri"] ?? "null"}). Please make sure an Azure DevOps collection URI is available", ex);
			}

			this.ProjectIdentifier = Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECTID") ?? configuration["MSTest:AzureDevOps:ProjectIdentifier"];
			if (string.IsNullOrEmpty(this.ProjectIdentifier))
			{
				throw new ArgumentNullException($"No project name or identifier/GUID was available either through the environment variable SYSTEM_TEAMPROJECTID ({Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECTID") ?? "null"}) or through the {settingsFile.FullName} file at MSTest:AzureDevOps:ProjectIdentifier ({configuration["MSTest:AzureDevOps:ProjectIdentifier"] ?? "null"}). Please make sure an Azure DevOps team project name is available");
			}
		}
	}

	public enum TokenType
	{
		Bearer,
		Basic
	}

}
