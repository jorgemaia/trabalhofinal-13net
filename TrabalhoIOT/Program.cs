using System;
using System.Threading;
using Microsoft.Azure.Devices.Client;

namespace TrabalhoIOT
{
    class Program
    {
        private static readonly string _deviceConnectionString = "HostName=;DeviceId=;SharedAccessKey=";
        private static TransportType _transportType = TransportType.Amqp;

        static void Main(string[] args)
        {
            Console.WriteLine("Digite start para iniciar e stop para parar");

            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString, _transportType);

            var monitor = new WeatherMonitor(deviceClient);

            while (true)
            {
                var comando = Console.ReadLine();
                if (comando == "stop")
                {
                    monitor.Stop();
                }
                if (comando == "start")
                {
                    ThreadPool.QueueUserWorkItem(BackgroundTask, monitor);
                }
            };
        }

        static void BackgroundTask(Object stateInfo)
        {
            WeatherMonitor monitor = (WeatherMonitor)stateInfo;
            monitor.Start();
        }
    }
}
