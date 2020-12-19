using System;
using System.Threading.Tasks;
using FunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

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
                log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

                var countInsertDocument = await InsertDocument();
                log.LogInformation("Add docs: {0}", countInsertDocument);
            }
            catch (Exception e)
            {
                log.LogInformation("Exception: {0}", e.Message);
                throw;
            }
        }

        private static async Task<int> InsertDocument(/*Document document*/)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnectionString");
            const string sqlExpression =
                "INSERT INTO Production.Document (DocumentNode, Title, Owner, FolderFlag, FileName, FileExtension, Revision, ChangeNumber, Status, DocumentSummary) VALUES " +
                "('/'+CAST((select count(DocumentNode) from Production.Document) AS VARCHAR(10))+'/', " +
                "@Title, @Owner, @FolderFlag, @FileName, @FileExtension, @Revision, @ChangeNumber, @Status, @DocumentSummary)";

            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand(sqlExpression, connection);

            command.Parameters.AddWithValue("@Title", "hey 3");
            command.Parameters.AddWithValue("@Owner", 220);
            command.Parameters.AddWithValue("@FolderFlag", 1);
            command.Parameters.AddWithValue("@FileName", "test code");
            command.Parameters.AddWithValue("@FileExtension", ".txt");
            command.Parameters.AddWithValue("@Revision", 0);
            command.Parameters.AddWithValue("@ChangeNumber", 0);
            command.Parameters.AddWithValue("@Status", 1);
            command.Parameters.AddWithValue("@DocumentSummary", "DocumentSummary");
            //command.Parameters.AddWithValue("@Document", value2);
            return command.ExecuteNonQuery();
        }
    }
}
