using SDX.FunctionsDemo.ImageProcessing;

namespace SDX.FunctionsDemo.FunctionApp
{
    public class ProcessImageMessage
    {
        public string ID { get; set; }
        public ImageType ImageType { get; set; }

        public ProcessImageMessage()
        {
        }

        public ProcessImageMessage(string id, ImageType imageType)
        {
            this.ID = id;
            this.ImageType = imageType;
        }
    }
}