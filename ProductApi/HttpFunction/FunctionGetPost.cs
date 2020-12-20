using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            switch (req.Method)
            {
                case "GET":
                {
                    var fileName = req.Query["filename"];
                    var document = await GetDocument(fileName);
                    
                    if (document == null || document.Length == 0 || string.IsNullOrEmpty(fileName))
                    {
                        return new NotFoundObjectResult(document);
                    }

                    var result = new FileContentResult(document, "application/octet-stream") { FileDownloadName = fileName };
                    return result;
                }
                case "POST":
                {
                    var fileName = "users.csv";
                    var document = GetDocument(fileName);
                    return new OkObjectResult(document);
                }
                default:
                {
                    string responseMessage = string.IsNullOrEmpty(name)
                        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                        : $"Hello, {name}. This HTTP triggered function executed successfully.";

                    return new OkObjectResult(responseMessage);
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
    }
}
