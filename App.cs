using AzureStoragePractice.Services;

namespace AzureStoragePractice
{
    public class App
    {

        private readonly IConfigurationService configuration;
        private readonly IBlobService blobService;

        public App(IConfigurationService _configuration, IBlobService _blobService)
        {
            // Dependency Injection
            configuration = _configuration;
            blobService = _blobService;
        }


        public async Task TestUpload()
        {
            // Testing the upload method
            var fileStream = File.OpenRead(@"gopher.png");
            await blobService.CreateAndEnablePublicAccess();

            await blobService.UploadAsync(fileStream);
            Console.ReadLine();
            await blobService.DeleteAsync();
        }
    }
}