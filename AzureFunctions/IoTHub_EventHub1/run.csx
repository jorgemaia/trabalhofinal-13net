#load "Item.csx"
using System;

public static Item Run(string myIoTHubMessage, ILogger log)
{
    log.LogInformation($"C# IoT Hub trigger function processed a message: {myIoTHubMessage}");
    
    return new Item() {PartitionKey = "some-partition-key", RowKey=Guid.NewGuid().ToString(), ItemName = myIoTHubMessage};

}