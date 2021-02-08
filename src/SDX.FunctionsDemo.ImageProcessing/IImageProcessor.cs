namespace SDX.FunctionsDemo.ImageProcessing
{
    public interface IImageProcessor
    {
        byte[] ProcessImage(byte[] data, ImageType imageType);
    }
}