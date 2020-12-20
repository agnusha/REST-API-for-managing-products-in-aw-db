using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;

namespace HttpFunction
{
    public static class FunctionGetPost
    {
        [FunctionName("FunctionGetPost")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                //	GET request – function should return a file from the AdventureWorks DB
                case "GET":
                    {
                        var fileName = req.Query["filename"];
                        var document = await GetDocument(fileName);

                        if (document == null || document.Length == 0 || string.IsNullOrEmpty(fileName))
                        {
                            log.LogInformation("Document doesn't exist.");
                            return new NotFoundObjectResult(document);
                        }

                        log.LogInformation("Function (POST) returned document successfully.");
                        var result = new FileContentResult(document, "application/octet-stream") { FileDownloadName = fileName };
                        return result;
                    }
                //	POST request – function should upload a file to blob and create a notification
                case "POST":
                    {
                        var file = req.Form.Files[0];

                        var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("ConnectionStrings:CloudStorageAccountConnectionString"));
                        var containerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("CloudStorageAccount:Container"));
                        var blobClient = containerClient.GetBlobClient(file.FileName);
                        await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
                        var queueClient = new QueueClient(Environment.GetEnvironmentVariable("ConnectionStrings:CloudStorageAccountConnectionString"), Environment.GetEnvironmentVariable("CloudStorageAccount:Queue"));

                        await queueClient.CreateIfNotExistsAsync();
                        if (await queueClient.ExistsAsync())
                        {
                            await queueClient.SendMessageAsync(Base64Encode($"Save file {blobClient.Name} in function with metadata - ContentType {file.ContentType}."));
                        }
                        
                        log.LogInformation("Function (POST) upload a file to blob and create a notification successfully.");
                        return new NoContentResult();

                    }
                default:
                    {
                        return new OkObjectResult("This HTTP triggered function executed.");
                    }
            }
        }


        private static async Task<byte[]> GetDocument(string fileName)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnectionString");
            var sqlExpression = $"SELECT Document FROM Production.Document WHERE FileName = '{fileName}'";

            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand(sqlExpression, connection);
            var document = (byte[])command.ExecuteScalar();

            return document;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
