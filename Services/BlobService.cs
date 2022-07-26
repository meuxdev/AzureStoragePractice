using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace AzureStoragePractice.Services
{

    public class BlobService : IBlobService
    {
        private BlobContainerClient blobContainer; // Container Blob Reference
        private CloudBlobContainer blobContainerDeprecated; // Container Blob Reference With the deprecated 

        private bool BlobContainerReady { get; set; }



        public BlobService(string connectionString, string containerName)
        {
            // Init Container 

            blobContainer = new BlobContainerClient(connectionString, containerName.ToLower());

            // Init Container Deprecated Way
            CloudStorageAccount accountStorage = CloudStorageAccount.Parse(connectionString); // Connect to the account storage
            CloudBlobClient blobClient = accountStorage.CreateCloudBlobClient(); // Create a new Blob Client
            blobContainerDeprecated = blobClient.GetContainerReference((containerName + "-deprecated").ToLower()); // get the container reference
            BlobContainerReady = true;
        }

        public async Task CreateAndEnablePublicAccess()
        {

            try
            {
                await EnsureCreatedAndEnablePublicAccessD();
            }
            catch (Microsoft.Azure.Storage.StorageException)
            {
                // error handle for the request if the container is dropping
                BlobContainerReady = false;
            }

            try
            {
                await EnsureCreatedAndEnablePublicAccess();
            }
            catch (Azure.RequestFailedException)
            {
                // error handle for the request if the container is dropping
                BlobContainerReady = false;
            }


        }

        private async Task EnsureCreatedAndEnablePublicAccess()
        {
            /*
                Inits the container if not existing
                    - Ensure this is created.
                    - Updates the Access policy
            */
            await blobContainer.CreateIfNotExistsAsync(); // verify 
            await blobContainer.SetAccessPolicyAsync(PublicAccessType.Blob); //update access policy
        }

        private async Task EnsureCreatedAndEnablePublicAccessD()
        {
            /*
                This function.
                <<< THIS USES THE DEPRECATED PACKAGE OF MICROSOFT. >>>
                    - Ensure this is created.
                    - Updates the Access policy
            */
            await blobContainerDeprecated.CreateIfNotExistsAsync(); // Ensure the container exist
            await blobContainerDeprecated.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }); // update access policy
        }

        public async Task UploadAsync(FileStream fileStream)
        {
            /*
                This functions receives a file : FileStream and sends the request to upload to the blob
                using async properties.
            */
            string randomGuid = Guid.NewGuid().ToString();

            if (BlobContainerReady)
            {
                // verify the container is ready for the upload
                await UploadBlobAsync(fileStream, randomGuid);
                await UploadBlobDeprecatedAsync(fileStream, randomGuid);
                Console.WriteLine("Images uploaded to cloud storage");
            }
            else
            {

                Console.WriteLine("Wait a few more seconds to send the request");
            }

        }

        private async Task UploadBlobDeprecatedAsync(FileStream fileStream, string randomGuid)
        {
            /*
                Upload Deprecated function.
                    - Receives a File Stream
                    - Receives a random Guid to name the image.
                    - Uploads the file to the container.
            */
            CloudBlockBlob blobDeprecated = blobContainerDeprecated.GetBlockBlobReference($"{randomGuid}.png");
            await blobDeprecated.UploadFromStreamAsync(fileStream);
            Console.WriteLine("Uploading Deprecated...");
        }

        private async Task UploadBlobAsync(FileStream fileStream, string randomGuid)
        {
            /*
                Upload function support. 
                    - Receives a File Stream
                    - Receives a random Guid to name the image.
                    - Uploads the file to the container.
            */
            BlobClient blob = blobContainer.GetBlobClient($"{randomGuid}.png");
            await blob.UploadAsync(fileStream);
            Console.WriteLine("Uploading...");
        }


        public async Task DeleteAsync()
        {
            /*
                Deletes the Blob container Note this can take a few seconds to be reflected
            */
            await blobContainer.DeleteIfExistsAsync();
            await blobContainerDeprecated.DeleteIfExistsAsync();
            Console.WriteLine("Dropping the blob container...");
        }
    }


    public interface IBlobService
    {
        Task UploadAsync(FileStream fileStream);

        Task CreateAndEnablePublicAccess();

        Task DeleteAsync();
    }
}