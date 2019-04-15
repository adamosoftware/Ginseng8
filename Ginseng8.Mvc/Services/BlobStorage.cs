﻿using Ginseng.Mvc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Services
{
	public class BlobStorageSettings
	{
		public string AccountName { get; set; }
		public string AccountKey { get; set; }
	}

	public class BlobStorage
	{
		private readonly IConfiguration _config;
		private readonly BlobStorageSettings _settings;

		public BlobStorage(IConfiguration config)
		{
			_config = config;
			_settings = config.GetSection("BlobStorage").Get<BlobStorageSettings>();
		}

		public string AccountName
		{
			get { return _settings.AccountName; }
		}

		public CloudStorageAccount GetAccount()
		{
			return new CloudStorageAccount(new StorageCredentials(AccountName, _settings.AccountKey), true);
		}

		public CloudBlobClient GetClient()
		{
			return GetAccount().CreateCloudBlobClient();
		}

		public static async Task<CloudBlobContainer> GetOrgContainerAsync(CloudBlobClient client, string orgName)
		{
			var container = client.GetContainerReference(orgName);
			await container.CreateIfNotExistsAsync();
			return container;
		}

		public async Task<CloudBlobContainer> GetOrgContainerAsync(string orgName)
		{
			var client = GetClient();
			return await GetOrgContainerAsync(client, orgName);
		}

		public string GetUrl(string orgName, string blobName)
		{
			return $"https://{AccountName}.blob.core.windows.net:443/{orgName}/{blobName}";
		}

		public async Task<IEnumerable<BlobInfo>> ListBlobs(string orgName, string prefix)
		{			
			var container = await GetOrgContainerAsync(orgName);
			BlobContinuationToken token = new BlobContinuationToken();
			List<BlobInfo> results = new List<BlobInfo>();
			do
			{
				var response = await container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.None, null, token, null, null);
				token = response.ContinuationToken;
				results.AddRange(response.Results.OfType<CloudBlockBlob>().Select(blob => new BlobInfo()
				{
					Uri = blob.Uri,
					Filename = Path.GetFileName(blob.Uri.ToString()),
					Length = blob.Properties.Length,
					LastModified = blob.Properties.LastModified
				}));
			} while (token != null);

			return results;
		}
	}
}