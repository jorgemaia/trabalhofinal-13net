using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace az_func
{
    public static class powerbi
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("powerbi")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "ConexaoIotHub")]EventData message, ILogger log)
        {
            var urlPowerBi = "https://api.powerbi.com/beta/ae590687-eb73-4179-a1d3-6ec11bb7a27d/datasets/a6d8c4e8-80b5-4d44-870a-c12f6fd05e86/rows?key=v3t50wmYqr2OUmDkYngdYJpcFiU%2FhDQ17n2YycI9CK2%2BxJhGSUGLeAAW1ET%2FXaDlz4G%2BNihVD0ZeLVh1%2FR3dFw%3D%3D";


            client.PostAsync(urlPowerBi, new StringContent(Encoding.UTF8.GetString(message.Body.Array), Encoding.UTF8, "application/json"));
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }
}