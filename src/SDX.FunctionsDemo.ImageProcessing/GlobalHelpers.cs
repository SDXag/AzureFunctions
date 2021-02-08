using System.IO;

namespace SDX.FunctionsDemo
{
    public static class GlobalHelpers
    {
        public static byte[] CopyToArray(this Stream stream)
        {
            if (stream == null)
                return null;
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}