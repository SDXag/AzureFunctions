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
    public class GetImage
    {
        private readonly IImageStorage _imageStorage;

        public GetImage(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        /// <summary>
        /// IN/Query:
        /// - "id"          : id (guid)
        /// - "imageType"   : imageType (string)
        /// OUT:
        /// - byte[]        : image data (image/png)
        /// </summary>
        [FunctionName("GetImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Übergebene Informationen aus Request auslesen
            string id = req.Query["id"];
            string size = req.Query["size"];
            string effect = req.Query["effect"];
            var imageType = ImageType.TryParse(size, effect);
            if (imageType == null)
                return new BadRequestResult();

            // Image suchen
            var imageName = ImageNameHelper.CreateImageName(id, imageType);
            var imageStream = await _imageStorage.DownloadImageBlobAsync(imageName);
            if (imageStream == null)
                return new NotFoundObjectResult(imageName);

            // Image liefern
            return new FileStreamResult(imageStream, ContentTypes.Png);
        }
    }
}
