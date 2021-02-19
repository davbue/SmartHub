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
    public interface IMqttHubClient
    {
        Task ConnectAsync();

        Task Subscribe(string topic);

        Task PublishMessage(string topic, string payload);

    }
}