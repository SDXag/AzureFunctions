namespace SDX.FunctionsDemo.ImageProcessing
{
    public class ImageProcessor : IImageProcessor
    {
        public byte[] ProcessImage(byte[] data, ImageType imageType)
        {
            Simulation.SimulateProcessing(imageType.Size);

            switch (imageType.Effect)
            {
                case Effect.GrayScale: return ImageSharpHelper.GrayScale(data, imageType.Size);
                case Effect.Sepia: return ImageSharpHelper.Sepia(data, imageType.Size);
                case Effect.OilPaint: return ImageSharpHelper.OilPaint(data, imageType.Size);
                case Effect.GaussianBlur: return ImageSharpHelper.GaussianBlur(data, imageType.Size);
                case Effect.RoundImage: return ImageSharpHelper.RoundImage(data, imageType.Size);
                default: return ImageSharpHelper.Resize(data, imageType.Size);
            }
        }
    }
}
