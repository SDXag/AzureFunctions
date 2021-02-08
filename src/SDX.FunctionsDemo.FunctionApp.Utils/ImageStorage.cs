using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

namespace SDX.FunctionsDemo.FunctionApp.Utils
{
    public class ImageStorage : IImageStorage
    {
        private readonly IConfiguration _configuration;

        public ImageStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task UploadImageBlobAsync(string fileName, byte[] data)
        {
            var container = await GetBlobContainerAsync(StorageDefines.Blobs.Images);
            var blob = container.GetBlobClient(fileName);
            var ms = new MemoryStream(data);
            await blob.UploadAsync(ms);
        }

        public async Task PostProcessImageAsync(string message)
        {
            var queue = await GetQueueAsync(StorageDefines.Queues.ProcessImage);

            // Message muss base64-encoded werden!
            var bytes = Encoding.UTF8.GetBytes(message);
            var msg = Convert.ToBase64String(bytes);
            await queue.SendMessageAsync(msg);
        }

        public async Task<Stream> DownloadImageBlobAsync(string fileName)
        {
            var container = await GetBlobContainerAsync(StorageDefines.Blobs.Images);
            var blob = container.GetBlobClient(fileName);
            var exists = await blob.ExistsAsync();
            if (!exists)
                return null;

            var download = await blob.DownloadAsync();
            return download.Value.Content;
        }

        async Task<QueueClient> GetQueueAsync(string queueName)
        {
            var connectionString = _configuration[StorageDefines.StorageConnectionString];
            var queue = new QueueClient(connectionString, queueName);
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
            return queue;
        }

        async Task<BlobContainerClient> GetBlobContainerAsync(string containerName)
        {
            var connectionString = _configuration[StorageDefines.StorageConnectionString];
            var container = new BlobContainerClient(connectionString, containerName);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            return container;
        }
    }
}
