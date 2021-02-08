using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SDX.FunctionsDemo.FunctionApp.Utils;
using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.FunctionApp
{
    public class ProcessImage
    {
        private readonly IImageStorage _imageStorage;
        private readonly IImageProcessor _imageProcessor;

        public ProcessImage(IImageStorage imageStorage, IImageProcessor imageProcessor)
        {
            _imageStorage = imageStorage;
            _imageProcessor = imageProcessor;
        }

        // [FunctionName("ProcessImage")]
        public async Task RunAsync(
            [QueueTrigger("processimage", Connection = "AzureWebJobsStorage")] ProcessImageMessage message,
            ILogger log)
        {
            // Übergebene Informationen aus Message auslesen
            var id = message.ID;
            var imageType = message.ImageType;
            log.LogInformation($"Verarbeite Image: id={id}; imageType={imageType}");

            // Orginal-Image lesen
            var originalImage = await GetOriginalImageAsync(id);

            // gewünschtes Image berechnen
            var newImage = _imageProcessor.ProcessImage(originalImage, imageType);

            // neues Image speichern
            await UploadImageAsync(log, id, imageType, newImage);
        }

        private async Task<byte[]> GetOriginalImageAsync(string id)
        {
            var originalImageName = ImageNameHelper.CreateImageName(id, ImageTypes.Original);
            var originalImageStream = await _imageStorage.DownloadImageBlobAsync(originalImageName);
            return originalImageStream.CopyToArray();
        }

        private async Task UploadImageAsync(ILogger log, string id, ImageType imageType, byte[] image)
        {
            var imageName = ImageNameHelper.CreateImageName(id, imageType);
            await _imageStorage.UploadImageBlobAsync(imageName, image);
            log.LogInformation($"Image bereitgestellt: id={id}; imageType={imageType}");
        }
    }
}
