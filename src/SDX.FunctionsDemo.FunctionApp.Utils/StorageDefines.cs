namespace SDX.FunctionsDemo.FunctionApp.Utils
{
    public class StorageDefines
    {
        /// <summary>Name des Connection-Strings; AzureWebJobsStorage ist der Default.</summary>
        public const string StorageConnectionString = "AzureWebJobsStorage";

        /// <summary>Namen von Blob-Containern.</summary>
        /// <remarks>
        /// Lowercase!
        /// https://docs.microsoft.com/en-us/rest/api/storageservices/Naming-and-Referencing-Containers--Blobs--and-Metadata#resource-names
        /// </remarks>
        public static class Blobs
        {
            public const string Images = "images";
        }

        /// <summary>Namen von Queues.</summary>
        /// <remarks>
        /// Lowercase!
        /// https://docs.microsoft.com/en-us/rest/api/storageservices/naming-queues-and-metadata
        /// </remarks>
        public static class Queues
        {
            public const string ProcessImage = "processimage";
        }
    }
}