using Azure.Storage.Blobs;
using Domain.ConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Utils.Azure
{
    public class AzureStorage : IAzureStorage
    {
        private readonly AzureConfigurations _azureConfigurations;
        private readonly IConfiguration _configuration;
        public AzureStorage(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureConfigurations = new AzureConfigurations();
            _configuration.Bind(_azureConfigurations.Section, _azureConfigurations);
        }
        public async Task<string> UploadAsync(IFormFile blob)
        {

            // Get a reference to a container named in appsettings.json and then create it
            BlobContainerClient container = new BlobContainerClient(_azureConfigurations.BlobConnectionString, _azureConfigurations.BlobContainerName);

            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = container.GetBlobClient(blob.FileName);

            // Open a stream for the file we want to upload
            await using (Stream? data = blob.OpenReadStream())
            {
                // Upload the file async
                await client.UploadAsync(data, overwrite: true);
            }

            return client.Uri.AbsoluteUri;
        }
        public async Task<bool> DeleteAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(_azureConfigurations.BlobConnectionString, _azureConfigurations.BlobContainerName);

            BlobClient file = client.GetBlobClient(blobFilename);

            // Delete the file
            var response = await file.DeleteAsync();

            return response.IsError ? false : true;
        }
    }
}
