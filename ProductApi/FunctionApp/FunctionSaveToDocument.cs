using System;
using System.Threading.Tasks;
using FunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public static class FunctionSaveToDocument
    {
        [FunctionName("FunctionSaveToDocument")]
        public static void Run([QueueTrigger("awfilequeue", Connection = "Queuetriggerconnectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }

        private static async Task<int> InsertDocument(/*Document document*/)
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
            Console.WriteLine("Add docs: {0}", number);

            return number;

        }
    }
}
