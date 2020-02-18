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

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            string value = req.Query["name"];
            string instance = await client.StartNewAsync<string>("myorchestrator", value);

            return client.CreateCheckStatusResponse(req, instance);

        }

        [FunctionName("myorchestrator")]
        public static async Task run1(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log
            ) {

            string value = context.GetInput<string>();

            context.CallActivityAsync<string>("firstactivity", value);
            context.CallActivityAsync<string>("secondactivity", value);

            string isapproved = await context.WaitForExternalEvent<string>("approval");
            if (isapproved == "yes") {
                string activitythree = await context.CallActivityAsync<string>("thirdactivity", value);
            }

        }

        [FunctionName("firstactivity")]
        public static async Task seerwer(
           [ActivityTrigger] IDurableActivityContext context,
           ILogger log
           )
        {
            log.LogInformation("activity 1 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            log.LogInformation(val.ToUpper());
           // return val.ToUpper();
        }

        [FunctionName("secondactivity")]
        public static async Task hjgfhgf(
           [ActivityTrigger] IDurableActivityContext context,
           ILogger log
           )
        {
            log.LogInformation("activity 2 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            log.LogInformation(val + " " + "second activity value");
           // return val + " " + "second activity value";
        }

        [FunctionName("thirdactivity")]
        public static async Task<string> thirdactivity(
           [ActivityTrigger] IDurableActivityContext context,
           ILogger log
           )
        {
            log.LogInformation("activity 3 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            log.LogInformation(val + " " + "third  activity value");
            return val + " " + "third activity value";
        }
    }
}
