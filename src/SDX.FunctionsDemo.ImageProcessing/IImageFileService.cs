using System.Threading.Tasks;

namespace SDX.FunctionsDemo.ImageProcessing
{
    public interface IImageFileService
    {
        Task<string> UploadImageAsync(string contentType, byte[] data);
        Task<byte[]> GetImageAsync(string id, ImageType imageType);
    }
}
