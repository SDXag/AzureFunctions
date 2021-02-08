using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SDX.FunctionsDemo.FunctionApp.Utils;
using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.FunctionApp
{
    public class UploadImage2
    {
        private readonly IImageStorage _imageStorage;

        public UploadImage2(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        /// <summary>
        /// IN/Header:
        /// - x-sdx-contentType : contentType (image/png)
        /// IN/Content:
        /// - byte array        : image data
        /// OUT:
        /// - string            : id
        /// </summary>
       // [FunctionName("UploadImage")]
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

            // Für jedes Format einen Queue-Eintrag schreiben, um die Berechnung asynchron anzustoßen
            foreach (var imageType in ImageTypes.Catalogue)
            {
                await RequestImageProcessingAsync(log, id, imageType);
            }

            return new OkObjectResult(id);
        }

        private async Task RequestImageProcessingAsync(ILogger log, string id, ImageType imageType)
        {
            var message = new ProcessImageMessage(id, imageType);
            var json = JsonConvert.SerializeObject(message);
            await _imageStorage.PostProcessImageAsync(json);
            log.LogInformation($"Image-Verarbeitung angefordert: id={id}; imageType={imageType}");
        }

        private async Task UploadImageAsync(ILogger log, string id, ImageType imageType, byte[] image)
        {
            var imageName = ImageNameHelper.CreateImageName(id, imageType);
            await _imageStorage.UploadImageBlobAsync(imageName, image);
            log.LogInformation($"Image bereitgestellt: id={id}; imageType={imageType}");
        }
    }
}