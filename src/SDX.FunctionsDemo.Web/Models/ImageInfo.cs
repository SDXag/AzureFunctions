using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.Web.Models
{
    public class ImageInfo
    {
        public ImageType ImageType { get; set; }
        public string Source { get; set; }

        public ImageInfo(ImageType imageType, string source = null)
        {
            this.ImageType = imageType;
            this.Source = source;
        }
    }
}