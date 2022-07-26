using AzureStoragePractice.Services;

namespace AzureStoragePractice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Init Services
            IConfigurationService configurationService = new ConfigurationService();
            IBlobService blobService = new BlobService(
                configurationService.GetConnectionString(),
                "sample-container");

            // Init App
            var app = new App(configurationService, blobService);

            // Test the upload
            await app.TestUpload();
        }
    }
}