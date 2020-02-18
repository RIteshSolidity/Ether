using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FunctionApp18
{
    public static class Function1
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = req.Query["name"];

            string instanceid = await client.StartNewAsync<string>("vsorchestrator1", requestBody);

            return client.CreateCheckStatusResponse(req, instanceid);
            
            
        }

        [FunctionName("vsorchestrator1")]
        public static async Task<string> asdasd(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log
            ) {
            string secondactivityvalue = null ;
            string incomingvalues = context.GetInput<string>();

                string firstactivityvalue = await context.CallActivityAsync<string>("firstactivity", incomingvalues);
                secondactivityvalue = await context.CallActivityAsync<string>("secondactivity", firstactivityvalue);

            return secondactivityvalue;
        }

        [FunctionName("firstactivity")]
        public static async Task<string> seerwer(
            [ActivityTrigger] IDurableActivityContext context,
            ILogger log
            ) {
            log.LogInformation("activity 1 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            return val.ToUpper();
        }

        [FunctionName("secondactivity")]
        public static async Task<string> hjgfhgf(
           [ActivityTrigger] IDurableActivityContext context,
           ILogger log
           )
        {
            log.LogInformation("activity 2 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            return val + " " + "second activity value";
        }
    }
}
