using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions
{
    public static class LerDadosIotHub
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("LerDadosIotHub")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "Conexao")]EventData message,[Table("TabDados", Connection = "AzureWebJobsStorage")]ICollector<Mensagem> tabela , ILogger log)
        {
            tabela.Add(new Mensagem(Encoding.UTF8.GetString(message.Body.Array)));
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }

    public class  Mensagem : TableEntity
    {
        public Mensagem(string payload)
        {
            this.PartitionKey = "MyIotHubTest";
            this.RowKey = System.Guid.NewGuid().ToString();
            this.Payload = payload;
        }
        public string Payload{ get; set;}


    }
}