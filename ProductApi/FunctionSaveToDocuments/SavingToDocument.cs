using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Configuration;
using FunctionSaveToDocuments.Models;
using System.Data.SqlClient;

namespace FunctionSaveToDocuments
{
    //Is triggered by notification in Azure Queue
    //Get file name from notification
    //Get file content from Azure Blob
    //Save file to DB Document table
    
    public class SavingToDocument
    {

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

        private async Task<int> InsertDocument(Document document)
        {
                const string connectionString =
                    @"Server=tcp:aw-vm-sql-server.database.windows.net,1433;Initial Catalog=aw-vm-sql_2020-12-12T18-24Z;Persist Security Info=False;User ID=agnia;Password=Epam2020Create;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                const string sqlExpression =
                    "INSERT INTO Production.Document (DocumentNode, Title, Owner, FolderFlag, FileName, FileExtension, Revision, ChangeNumber, Status, DocumentSummary) VALUES " +
                    "('/'+CAST((select count(DocumentNode) from Production.Document) AS VARCHAR(10))+'/', " +
                    "@Title, @Owner, @FolderFlag, @FileName, @FileExtension, @Revision, @ChangeNumber, @Status, @DocumentSummary)";

                await using var connection = new SqlConnection(connectionString);
                connection.Open();
                var command = new SqlCommand(sqlExpression, connection);

                command.Parameters.AddWithValue("@Title", "hey 2");
                command.Parameters.AddWithValue("@Owner", 220);
                command.Parameters.AddWithValue("@FolderFlag", 1);
                command.Parameters.AddWithValue("@FileName", "test code");
                command.Parameters.AddWithValue("@FileExtension", ".txt");
                command.Parameters.AddWithValue("@Revision", 0);
                command.Parameters.AddWithValue("@ChangeNumber", 0);
                command.Parameters.AddWithValue("@Status", 1);
                command.Parameters.AddWithValue("@DocumentSummary", "DocumentSummary");
                //command.Parameters.AddWithValue("@Document", value2);

                var number = command.ExecuteNonQuery();
                Console.WriteLine("Добавлено документов: {0}", number);

            return number;

        }

        private static async Task<byte[]> DownloadFileFromBlobAsync(string blobReferenceKey)
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

    }
}
