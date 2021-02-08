using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.Web.Services
{
    public class FunctionAppImageFileService : IImageFileService
    {
        private const string UrlUploadImage = "/api/UploadImage";
        private const string UrlGetImage = "/api/GetImage";

        static HttpClient _client = new HttpClient();
        private readonly ILogger<FunctionAppImageFileService> _logger;
        private readonly IOptions<FunctionAppOptions> _options;

        public FunctionAppImageFileService(ILogger<FunctionAppImageFileService> logger, IOptions<FunctionAppOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        async Task<string> IImageFileService.UploadImageAsync(string contentType, byte[] data)
        {
            try
            {
                var o = _options.Value;
                var baseUrl = o.Url;
                var requestUri = baseUrl + UrlUploadImage;

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                var content = new ByteArrayContent(data);
                content.Headers.Add("x-sdx-contentType", contentType);
                request.Content = content;

                #region f.k.
                // functionsKey setzen
                AddFunctionKey(request.Headers, "UploadImage");
                #endregion

                _logger.LogInformation("Sende Image an " + baseUrl);
                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Fehler: " + response.StatusCode);
                    return null;
                }
                else
                {
                    _logger.LogInformation("Image gesendet.");
                    var id = await response.Content.ReadAsStringAsync();
                    return id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadImageAsync");
                return null;
            }
        }

        async Task<byte[]> IImageFileService.GetImageAsync(string id, ImageType imageType)
        {
            try
            {
                var o = _options.Value;
                var baseUrl = o.Url;
                var requestUri = baseUrl + UrlGetImage + $"?id={id}&size={imageType.Size}&effect={imageType.Effect}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                #region f.k.
                // Neu: functionsKey setzen
                AddFunctionKey(request.Headers, "UploadImage");
                #endregion

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetImageAsync");
                return null;
            }
        }

        #region f.k.
        private void AddFunctionKey(HttpHeaders headers, string functionName)
        {
            var o = _options.Value;
            var functionsKey = GetFunctionKey(o, functionName);
            if (!string.IsNullOrEmpty(functionsKey))
                headers.Add("x-functions-key", functionsKey);
        }

        private string GetFunctionKey(FunctionAppOptions options, string functionName)
        {
            if (options.Keys == null)
                return null;
            if (options.Keys.TryGetValue(functionName, out var key))
                return key;
            options.Keys.TryGetValue("default", out key);
            return key;
        }
        #endregion
    }
}
