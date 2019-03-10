// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace simulatedDevice
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        private const string s_connectionString = "HostName=AulaIot.azure-devices.net;DeviceId=DevicePiscina;SharedAccessKey=pWf1/a/6zjRdepDypGT3kJTzsj/+9miZ1ljOeGS541Q=";

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            var device = Guid.NewGuid().ToString();
            
            // Initial telemetry values
            double ph = 6.8;
            double min_pH = 7; //ideal entre 7 e 7,4
            double max_pH = 7.4;

            double alcalinidade = 70;
            double min_alcalinidade = 80; //ideal entre 80 e 120
            double max_alcalinidade = 120;

            double cloro = 0.5;
            double min_cloro = 1; //ideal entre 1 e 3
            double max_cloro = 3;

            Random rand = new Random();

            while (true)
            {
                double current_pH = ph + rand.NextDouble();
                double current_alcalinidade  = alcalinidade + (rand.NextDouble() * 70);
                double current_cloro  = cloro + (rand.NextDouble() * 3);

                // Create JSON message
                var telemetryDataPoint = new
                {
                    deviceId = device,
                    ph = current_pH,
                    alcalinidade = current_alcalinidade,
                    cloro = current_cloro
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("phAlert", (current_pH < min_pH)||(current_pH > max_pH) ? "true" : "false");
                message.Properties.Add("alcalinidadeAlert", (current_alcalinidade < min_alcalinidade)||(current_alcalinidade > max_alcalinidade) ? "true" : "false");
                message.Properties.Add("cloroAlert", (current_cloro < min_cloro)||(current_cloro > max_cloro) ? "true" : "false");

                // Send the tlemetry message
                await s_deviceClient.SendEventAsync(message).ConfigureAwait(false);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(10000).ConfigureAwait(false);
            }
        }

        private static void Main()
        {
            Console.WriteLine("IoT Hub Quickstarts - Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
