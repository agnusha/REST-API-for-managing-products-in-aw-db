using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionSaveToDocuments
{
    public static class SaveToDocument
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("queue-items", Connection = "queuetriggerconnectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
