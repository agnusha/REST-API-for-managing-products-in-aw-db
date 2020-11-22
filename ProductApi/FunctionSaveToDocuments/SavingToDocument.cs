using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Configuration;

namespace FunctionSaveToDocuments
{
    public static class SavingToDocument
    {

        public static async Task<byte[]> DownloadFileFromBlobAsync(string blobReferenceKey)
        {
            var connectionString = ConfigurationManager.AppSettings["Queuetriggerconnectionstring"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("awfilecontainer");
            var blockBlob = container.GetBlockBlobReference(blobReferenceKey);
            using var ms = new MemoryStream();
            if (await blockBlob.ExistsAsync())
            {
                await blockBlob.DownloadToStreamAsync(ms);
            }
            return ms.ToArray();
        }


        [FunctionName("FunctionSaveToDocument")]
        public static async Task RunAsync([QueueTrigger("awfilequeue", Connection = "Queuetriggerconnectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var bytes = await DownloadFileFromBlobAsync("photo_2020-10-06_21-13-41.jpg");
            Console.WriteLine(bytes.Length);
        }
    }
}
