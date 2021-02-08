using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SDX.FunctionsDemo.ImageProcessing;
using SDX.FunctionsDemo.Web.Models;
using SDX.FunctionsDemo.Web.Services;

namespace SDX.FunctionsDemo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImageFileService _imageFileService;
        private readonly IMemoryCache _cache;
        private readonly IOptions<FunctionAppOptions> _options;

        public HomeController(IOptions<FunctionAppOptions> options, IImageFileService imageFileService, IMemoryCache cache)
        {
            _imageFileService = imageFileService;
            _cache = cache;
            _options = options;
        }

        public IActionResult Index()
        {
            this.ViewData["IImageFileService"] = _options.Value.Url ?? "InMemory";
            return View();
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var start = DateTime.Now;
            var file = files.FirstOrDefault();

            #region Eingabevalidierung
            // Eingabevalidierung
            if (file == null)
            {
                this.ViewData.SetMessage("warning", "Keine Datei angegeben!");
                return View(nameof(Index));
            }
            if (file.ContentType != ContentTypes.Png)
            {
                this.ViewData.SetMessage("error", "Es werden nur .png-Dateien unterstützt!");
                return View(nameof(Index));
            }
            #endregion

            // Image zur Berechnung bereitstellen
            var data = file.CopyToArray();
            var id = await _imageFileService.UploadImageAsync(file.ContentType, data);
            if (string.IsNullOrEmpty(id))
            {
                this.ViewData.SetMessage("danger", "Der Upload konnte nicht durchgeführt werden!");
                return View(nameof(Index));
            }

            // Auf die Darstellungsseite wechseln
            return RedirectToAction(nameof(Images), new { id, start });
        }

        public async Task<IActionResult> Images(string id, DateTime start)
        {
            // alle Image Infos ermitteln
            var imageInfos = await GetImageInfosAsync(id);

            // Status aufbereiten
            var available = imageInfos.Where(ii => ii.Source != null).Count();
            if (available < imageInfos.Length)
            {
                // Hinweis und auto-refresh!
                int refresh = 2;
                this.ViewData.SetMessage("warning", $"Die Verarbeitung ist noch unvollständig: {available}/{imageInfos.Length}; Refresh nach {refresh} Sekunden.");
                this.ViewData["refresh"] = refresh;
            }
            else
            {
                var duration = DateTime.Now - start;
                this.ViewData.SetMessage("success", "Alle Bilder wurden verarbeitet! Dauer: " + Math.Round(duration.TotalSeconds) + " sec.");
            }

            // und anzeigen
            this.ViewData.SetImages(imageInfos);
            return View();
        }

        private async Task<ImageInfo[]> GetImageInfosAsync(string id)
        {
            var imageInfos = new List<ImageInfo>();
            foreach (var imageType in ImageTypes.Catalogue)
            {
                var image = await GetImageAsync(id, imageType);
                var imageInfo = CreateImageInfo(imageType, image);
                imageInfos.Add(imageInfo);
            }

            return imageInfos.ToArray();
        }

        private async Task<byte[]> GetImageAsync(string id, ImageType imageType)
        {
            var key = id + " " + imageType;
            _cache.TryGetValue<byte[]>(key, out var image);
            if (image == null)
            {
                image = await _imageFileService.GetImageAsync(id, imageType);
                _cache.Set(key, image);
            }
            return image;
        }

        private ImageInfo CreateImageInfo(ImageType imageType, byte[] image)
        {
            // Images werden nicht als Link, sondern embedded angezeigt:
            //      <img alt="..." src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIA..." />
            var info = new ImageInfo(imageType);
            if (image != null)
                info.Source = "data:image/png;base64," + Convert.ToBase64String(image);
            return info;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
