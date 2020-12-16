using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Configuration;
using FunctionSaveToDocuments.Models;

namespace FunctionSaveToDocuments
{
    //Is triggered by notification in Azure Queue
    //Get file name from notification
    //Get file content from Azure Blob
    //Save file to DB Document table
    
    public class SavingToDocument
    {
        private readonly AdventureWorks2019Context _context;
        public SavingToDocument(AdventureWorks2019Context context)
        {
            _context = context;
        }

        public async Task<Document> PostDocument(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

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
            //var bytes = await DownloadFileFromBlobAsync("photo_2020-10-06_21-13-41.jpg");
            var connectionString = "DefaultEndpointsProtocol = https; AccountName = awfile; AccountKey = HDeYG5nDvSsUABpHgp2UJXOcf7z8PQxibgdU4DgchYc2R2MXtXKfZ3aXRaZ0IVSlCVpBy4hoCif / TRKsHnNBgA ==; EndpointSuffix = core.windows.net";
            //var storageAccount = CloudStorageAccount.Parse(connectionString);
            //var blobClient = storageAccount.CreateCloudBlobClient();
            //var container = blobClient.GetContainerReference("awfilecontainer");
            //var blockBlob = container.GetBlockBlobReference("photo_2020-10-06_21-13-41.jpg");
            //using var ms = new MemoryStream();
            //if (await blockBlob.ExistsAsync())
            //{
            //    await blockBlob.DownloadToStreamAsync(ms);
            //}

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            // Get Blob Container  
            var container = blobClient.GetContainerReference("awfilecontainer");
            // Get reference to blob (binary content)  
            var blockBlob = container.GetBlockBlobReference("photo_2020-10-06_21-13-41.jpg");
            // Read content  
            using (var ms = new MemoryStream())
            {
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DownloadToStreamAsync(ms);
                }
                var bytes = ms.ToArray();
                Console.WriteLine(bytes.Length);
            }
        }
    }
}
