using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using SmartHub.Clients;
using SmartHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient mqttClient;
        private IMqttClientOptions options;
        private InfluxDBClient influxDBClient;

        public MqttClientService(IMqttClientOptions options)
        {
            this.options = options;
            influxDBClient = new InfluxDBClient();
            mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            mqttClient.ConnectedHandler = this;
            mqttClient.DisconnectedHandler = this;
            mqttClient.ApplicationMessageReceivedHandler = this;
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string temp = eventArgs.ApplicationMessage.Topic.Split('/').Last();
            if (eventArgs.ApplicationMessage.Topic.Split('/').Last() == "data")
            {
                MqttDataMessage mqttMessage = JsonConvert.DeserializeObject<MqttDataMessage>(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));
                if (mqttMessage.DataEntities.Any())
                {
                    foreach (DataEntity dataEntity in mqttMessage.DataEntities)
                    {
                        influxDBClient.WritePoint(dataEntity);
                    }
                }
                return Task.CompletedTask;
            }
            else if (eventArgs.ApplicationMessage.Topic.Split('/').Last() == "availability")
            {
                MqttAvailabilityMessage mqttMessage = JsonConvert.DeserializeObject<MqttAvailabilityMessage>(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));
                var context = new SmartHomeContext();
                var bool1 = !context.Gateways.Find(mqttMessage.GatewayId).Online;
                var bool2 = mqttMessage.Online;
                if (!context.Gateways.Find(mqttMessage.GatewayId).Online && mqttMessage.Online)
                {
                    List<Device> devices = context.Devices.Where(d => d.GatewayId == mqttMessage.GatewayId).ToList();
                    MqttConfigMessage configMessage = new MqttConfigMessage
                    {
                        GatewayId = mqttMessage.GatewayId,
                        DeviceConfigs = new List<DeviceConfig>()
                    };
                    foreach (Device device in devices)
                    {
                        DeviceConfig deviceConfig = new DeviceConfig
                        {
                            DeviceId = device.DeviceId,
                            DeviceTypeId = device.DeviceTypeId,
                            Pins = device.Pins.Split(',').Select(Int32.Parse).ToList(),
                            Enabled = device.Enabled
                        };

                        configMessage.DeviceConfigs.Add(deviceConfig);

                        
                    }
                    string payload = JsonConvert.SerializeObject(configMessage);
                    var task = PublishMessageAsync($"{mqttMessage.GatewayId}/update/config", payload);
                    task.Wait();
                    return Task.CompletedTask;
                }
                context.Gateways.Find(mqttMessage.GatewayId).Online = mqttMessage.Online;
                context.SaveChanges();
            }
            else
            {
                throw new System.NotImplementedException();
            }
                return Task.CompletedTask;
        }
            

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            System.Console.WriteLine("connected");
            var context = new SmartHomeContext();
            foreach(Gateway gateway in context.Gateways)
            {
                await mqttClient.SubscribeAsync($"{gateway.GatewayId}/state/data");
                await mqttClient.SubscribeAsync($"{gateway.GatewayId}/state/availability");
            }
        }

        public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
            return Task.CompletedTask;
            
        }

        public async Task PublishMessageAsync(string topic, string payload)
        {

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await mqttClient.ConnectAsync(options);
            if (!mqttClient.IsConnected)
            {
                await mqttClient.ReconnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                var disconnectOption = new MqttClientDisconnectOptions
                {
                    ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "NormalDiconnection"
                };
                await mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
            }
            await mqttClient.DisconnectAsync();
        }
    }
}
