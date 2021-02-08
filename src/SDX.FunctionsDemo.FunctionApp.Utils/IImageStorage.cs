using System.IO;
using System.Threading.Tasks;

namespace SDX.FunctionsDemo.FunctionApp.Utils
{
    public interface IImageStorage
    {
        Task UploadImageBlobAsync(string fileName, byte[] data);
        Task PostProcessImageAsync(string message);
        Task<Stream> DownloadImageBlobAsync(string fileName);
    }
}