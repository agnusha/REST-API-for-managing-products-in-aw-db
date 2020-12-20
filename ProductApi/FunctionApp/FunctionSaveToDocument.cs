using System;
using System.IO;
using System.Threading.Tasks;
using FunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace FunctionApp
{
    public static class FunctionSaveToDocument
    {
        //Is triggered by notification in Azure Queue
        //Get file name from notification
        //Get file content from Azure Blob
        //Save file to DB Document table

        [FunctionName("FunctionSaveToDocument")]
        public static async Task Run([QueueTrigger("awfilequeue", Connection = "Queuetriggerconnectionstring")]string myQueueItem, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Queue trigger function processed for message: {myQueueItem}");
                
                var fileName = myQueueItem.Split(new[] { ' ' })[2];
                var bytes = await DownloadFileFromBlobAsync(fileName);

                var doc = new TDocument(fileName, fileName.Split(new[] { '.' })[1])
                {
                    Title = fileName.Split(new[] { '.' })[0],
                    DocumentSummary = "Saved by azure function",
                    Document = bytes,
                };

                var countInsertDocument = await InsertDocument(doc);

                log.LogInformation("Add docs to database: {0}", countInsertDocument);

            }
            catch (Exception e)
            {
                log.LogInformation("Exception: {0}", e.Message);
                throw;
            }
        }

        private static async Task<int> InsertDocument(TDocument document)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnectionString");
            const string sqlExpression =
                "INSERT INTO Production.Document (DocumentNode, Title, Owner, FolderFlag, FileName, FileExtension, Revision, ChangeNumber, Status, DocumentSummary, Document) VALUES " +
                "('/'+CAST((select count(DocumentNode) from Production.Document) AS VARCHAR(10))+'/', " +
                "@Title, 220, 1, @FileName, @FileExtension, 0, 0, 1, @DocumentSummary, @Document)";

            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand(sqlExpression, connection);

            command.Parameters.AddWithValue("@Title", document.Title);
            command.Parameters.AddWithValue("@FileName", document.FileName);
            command.Parameters.AddWithValue("@FileExtension", document.FileExtension);
            command.Parameters.AddWithValue("@DocumentSummary", document.DocumentSummary);
            command.Parameters.AddWithValue("@Document",  document.Document);
            return command.ExecuteNonQuery();
        }

        private static async Task<byte[]> DownloadFileFromBlobAsync(string blobReferenceKey)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:CloudStorageAccountConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("awfilecontainer");
            var blockBlob = container.GetBlockBlobReference(blobReferenceKey);
            await using var ms = new MemoryStream();
            if (await blockBlob.ExistsAsync())
            {
                await blockBlob.DownloadToStreamAsync(ms);
            }
            return ms.ToArray();
        }
    }
}
