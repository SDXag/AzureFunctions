using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SDX.FunctionsDemo.ImageProcessing;
using SDX.FunctionsDemo.Web.Models;

namespace SDX.FunctionsDemo.Web.Controllers
{
    internal static class Helpers
    {
        public static void SetMessage(this ViewDataDictionary viewDataDictionary, string messageType, string message)
        {
            viewDataDictionary["messageType"] = messageType;
            viewDataDictionary["message"] = message;
        }

        public static void SetImages(this ViewDataDictionary viewDataDictionary, ImageInfo[] images)
        {
            viewDataDictionary["images"] = images;
        }

        public static byte[] CopyToArray(this IFormFile file)
        {
            using (var strm = file.OpenReadStream())
            {
                return strm.CopyToArray();
            }
        }
    }
}
