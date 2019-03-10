using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Functions
{
    public static class LerDadosIotHubPiscina
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("LerDadosIotHubPiscina")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "Conexao")]EventData message,[Table("TabDadosPiscina",Connection = "AzureWebJobsStorage")]ICollector<Mensagem> tabela, ILogger log)
        {
            //caso necessário salvar a mensagem em storage descomentar a linha abaixo
            //tabela.Add(new Mensagem(Encoding.UTF8.GetString(message.Body.Array)));
            
            //alertas de valores fora dos parametros
            var indicesAnalisadosString = Encoding.UTF8.GetString(message.Body.Array);
            var indicesAnalisados = JsonConvert.DeserializeObject<IndicesAnalisados>(indicesAnalisadosString);
            IndicesAnalisados.IndicesDentroDosParametros(indicesAnalisados);


            //Envio de dados para o powerBI
            var ulrpowerBi = "https://api.powerbi.com/beta/3f5f1a44-17e0-4dd7-8c18-ffc0fb28db36/datasets/cd76272c-00b1-48ed-9556-3f48379d6f58/rows?key=0Hdd%2F2%2F3i1FIsPiT8oosYj9cvTXfU2GmBmUTpJ%2FL%2B83jhm88%2BFVu2siML2hjkwgulaqEduvIlyvlCgTW99K8%2FA%3D%3D";
            log.LogInformation(ulrpowerBi);            
            client.PostAsync(ulrpowerBi,new StringContent(Encoding.UTF8.GetString(message.Body.Array),Encoding.UTF8,"application/json"));
            
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }        
    }

    public class IndicesAnalisados
    {
        #region valores de aceitação dos indices
        private const double minPh =7;
        private const double maxPh =7.4;

        private const double minAlcalinidade =80;
        private const double maxAlcalinidade =120;
        
        private const double minCloro =1;
        private const double maxCloro =3;
        #endregion
        public IndicesAnalisados(string dispositivo, double ph, double alcalinidade, double cloro)
        {
            this.dispositivo = dispositivo;
            this.ph = ph;
            this.alcalinidade = alcalinidade;
            this.cloro = cloro;
        }
        public string dispositivo {get; private set;}
        public double ph {get;private set;}
        public double alcalinidade {get;private set;}
        public double cloro {get;private set;}

        public bool phForaDaNormalidade()
        {
            return ph<minPh || ph>maxPh;
        }

        public bool alcalinidadeForaDaNormalidade()
        {
            return alcalinidade<minAlcalinidade || alcalinidade>maxAlcalinidade;
        }

        public bool cloroForaDaNormalidade()
        {
            return cloro<minCloro || cloro>maxCloro;
        }

        public static void IndicesDentroDosParametros(IndicesAnalisados indicesAnalisados)
        {
            var valores ="Indice(s) fora dos parametros: ";
            var envia=false;

            if(indicesAnalisados.phForaDaNormalidade())
            {
                valores+=$"\nph: {indicesAnalisados.ph.ToString()}";
                envia=true;
            }

            if(indicesAnalisados.alcalinidadeForaDaNormalidade())
            {
                valores+=$"\nalcalinidade: {indicesAnalisados.alcalinidade.ToString()}";
                envia=true;
            }

            if(indicesAnalisados.cloroForaDaNormalidade())
            {
                valores+=$"\ncloro: {indicesAnalisados.cloro.ToString()}";
                envia=true;
            }
            
            if(envia)
            {
                //publica em API para ser consumido atraves do app
                // var client = new HttpClient();
                // var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(indicesAnalisados));
                // var content = new ByteArrayContent(byteData);
                // content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // client.PostAsync("/PublisherProductionAreaChanged",content);                
            }
        }
    }
    public class Mensagem:TableEntity
    {
        public Mensagem(string payload)
        {
            this.Payload =payload;
            this.PartitionKey = "MeuTesteIoTHub";
            this.RowKey = System.Guid.NewGuid().ToString();
        }
        public string Payload {get;set;}
    }


}