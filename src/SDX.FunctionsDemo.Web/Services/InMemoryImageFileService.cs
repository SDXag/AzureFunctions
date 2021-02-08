using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.Web.Services
{
    public class InMemoryImageFileService : IImageFileService
    {
        private static readonly Dictionary<string, Dictionary<ImageType, byte[]>> _images = new Dictionary<string, Dictionary<ImageType, byte[]>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<InMemoryImageFileService> _logger;
        private readonly IImageProcessor _imageProcessor;

        public InMemoryImageFileService(ILogger<InMemoryImageFileService> logger, IImageProcessor imageProcessor)
        {
            _logger = logger;
            _imageProcessor = imageProcessor;
        }

        Task<string> IImageFileService.UploadImageAsync(string contentType, byte[] data)
        {
            var images = new Dictionary<ImageType, byte[]>();
            images[ImageTypes.Original] = data;

            // alle Images berechnen...
            foreach (var imageType in ImageTypes.Catalogue)
            {
                _logger.LogInformation("Processing " + imageType + " ...");
                images[imageType] = _imageProcessor.ProcessImage(data, imageType);
            }

            // und in _images ablegen
            var id = Guid.NewGuid().ToString();
            _images[id] = images;
            return Task.FromResult(id);
        }

        Task<byte[]> IImageFileService.GetImageAsync(string id, ImageType imageType)
        {
            if (!_images.TryGetValue(id, out var images))
                return Task.FromResult((byte[])null);

            if (!images.TryGetValue(imageType, out var data))
                return Task.FromResult((byte[])null);

            return Task.FromResult(data);
        }
    }
}
