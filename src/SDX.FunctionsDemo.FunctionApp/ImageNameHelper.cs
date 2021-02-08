using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.FunctionApp.Utils
{
    /// <summary>Diese Klasse hilft, die Namenskonvention für die abgelegten Bilder einzuhalten.</summary>
    static class ImageNameHelper
    {
        public static string CreateImageName(string id, ImageType imageType)
        {
            return $"{id}-{imageType}.png";
        }
    }
}
