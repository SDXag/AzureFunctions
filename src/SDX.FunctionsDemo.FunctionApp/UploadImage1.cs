using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SDX.FunctionsDemo.FunctionApp.Utils;
using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.FunctionApp
{
    public class UploadImage1
    {
        private readonly IImageStorage _imageStorage;
        private readonly IImageProcessor _imageProcessor;

        public UploadImage1(IImageStorage imageStorage, IImageProcessor imageProcessor)
        {
            _imageStorage = imageStorage;
            _imageProcessor = imageProcessor;
        }

        /// <summary>
        /// IN/Header:
        /// - x-sdx-contentType : contentType (image/png)
        /// IN/Content:
        /// - byte array        : image data
        /// OUT:
        /// - string            : id
        /// </summary>
        [FunctionName("UploadImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Übergebene Informationen aus Request auslesen
            string contentType = req.Headers["x-sdx-contentType"];
            var originalImage = req.Body.CopyToArray();

            // Eingabevalidierung
            if (contentType != ContentTypes.Png)
                return new BadRequestObjectResult("invalid content type: " + contentType);

            // Orginal-Image speichern
            var id = Guid.NewGuid().ToString();
            await UploadImageAsync(log, id, ImageTypes.Original, originalImage);

            // Images berechnen und speichern
            foreach (var imageType in ImageTypes.Catalogue)
            {
                var newImage = _imageProcessor.ProcessImage(originalImage, imageType);
                await UploadImageAsync(log, id, imageType, newImage);
            }

            // ID als Eregbnis liefern
            return new OkObjectResult(id);
        }

        private async Task UploadImageAsync(ILogger log, string id, ImageType imageType, byte[] image)
        {
            var imageName = ImageNameHelper.CreateImageName(id, imageType);
            await _imageStorage.UploadImageBlobAsync(imageName, image);
            log.LogInformation($"Image bereitgestellt: id={id}; imageType={imageType}");
        }
    }
}
