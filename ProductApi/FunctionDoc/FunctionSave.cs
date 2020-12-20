using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionDoc
{
    public static class FunctionSave
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "QueueTriggerConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
