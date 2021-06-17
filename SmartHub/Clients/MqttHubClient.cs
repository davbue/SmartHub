using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions;
using Newtonsoft.Json;
using SmartHub.Clients;
using SmartHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Clients
{
    public class MqttHubClient
    {
        private IMqttClient Client;
        private InfluxDBClient InfluxDBClient;
        /// &lt;summary&gt;
        /// Connect to broker.
        /// &lt;/summary
        /// &lt;returns&gt;Task.&lt;/returns
        /// 

        public MqttHubClient()
        {
            MqttFactory factory = new MqttFactory();
            Client = factory.CreateMqttClient();
            InfluxDBClient = new InfluxDBClient();
        }
        


        public void ConfigureListening()
        {
            Client.UseApplicationMessageReceivedHandler(e =>
            {
                //Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                //Console.WriteLine();
                if (e.ApplicationMessage.Topic.Split('/').Last() == "Data")
                {
                    MqttDataMessage mqttMessage = JsonConvert.DeserializeObject<MqttDataMessage>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    foreach (DataEntity dataEntity in mqttMessage.DataEntities)
                    {
                        InfluxDBClient.WritePoint(dataEntity);
                    }
                }
            });
        }
        public async Task ConnectAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("192.168.43.62", 1883) // Port is optional
                .Build();
            await Client.ConnectAsync(options, CancellationToken.None);

            Client.UseDisconnectedHandler(async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await Client.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    //Reconnection failed
                }
            });
        }
        public async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
        }

        public async Task Subscribe(string topic)
        {
            await Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
        }

        public async Task PublishMessage(string topic, string payload)
        {
            
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await Client.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }

    }
}