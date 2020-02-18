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
using System.Threading;

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


        [FunctionName("Function2")]
        public static async Task<IActionResult> Run1(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    [DurableClient] IDurableOrchestrationClient client,
    ILogger log)
        {
            string instancei = req.Query["instanceid"];
            string eventname = req.Query["eventname"];
            string eventvalue = req.Query["eventvalue"];
            await client.RaiseEventAsync(instancei, eventname, eventvalue);



            return new OkObjectResult("ok");

        }

        [FunctionName("myorchestrator")]
        public static async Task run1(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log
            ) {

            string value = context.GetInput<string>();

            Task<string> firstactivityresult = null;
            Task<string> secondactivityresult = null;
            Task<string> waitResult = null;
            string activitythree = null;
            using (CancellationTokenSource source = new CancellationTokenSource()) {

                var expiryTime = context.CurrentUtcDateTime.AddSeconds(100);
                var timer = context.CreateTimer(expiryTime, source.Token);
                if (!context.IsReplaying) {
                }
                firstactivityresult = context.CallActivityAsync<string>("firstactivity", value);
                secondactivityresult = context.CallActivityAsync<string>("secondactivity", value);
                
                waitResult = context.WaitForExternalEvent<string>("createvm");

                var result = await Task.WhenAny(timer, waitResult);

                if (result == waitResult)
                {
                    source.Cancel();
                    activitythree  = await context.CallActivityAsync<string>("thirdactivity", value);

                }
                else {
                    activitythree = await context.CallActivityAsync<string>("thirdactivity", "werwerwerwer");
                }
            }


        }

        [FunctionName("firstactivity")]
        public static async Task<string> seerwer(
           [ActivityTrigger] IDurableActivityContext context,
           ILogger log
           )
        {
            log.LogInformation("activity 1 function invoked");
            string val = context.GetInput<string>();
            await Task.Delay(100);
            log.LogInformation(val.ToUpper());
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
            log.LogInformation(val + " " + "second activity value");
            return val + " " + "second activity value";
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
