using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace TrabalhoIOT
{
    public class WeatherMonitor
    {
        private static bool StopFlag = false;
        private DeviceClient _deviceClient;
        private static Random _randomGenerator = new Random();

        public WeatherMonitor(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public Task Start()
        {
            Console.WriteLine("Monitor Climatico está ligado");

            StopFlag = true;

            while (StopFlag)
            {
                float Temperature = _randomGenerator.Next(20, 35);
                float Humidity = _randomGenerator.Next(60, 80);

                string payload = $"{{\"data\":{DateTime.Now},\"temperature\":{Temperature},\"humidity\":{Humidity}}}";

                Message eventMessage = new Message(Encoding.UTF8.GetBytes(payload));

                Console.WriteLine($"Enviando dado: {payload}");

                _deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);

                Thread.Sleep(1000);
            }

            Console.WriteLine("Monitor Climatico está parado");

            Console.WriteLine("Digite start para iniciar e stop para parar");

            return Task.CompletedTask;
        }

        public void Stop()
        {
            StopFlag = false;
        }
    }
}
