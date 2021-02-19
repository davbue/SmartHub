using Microsoft.Extensions.Hosting;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using System.Threading.Tasks;

namespace SmartHub.Services
{
    public interface IMqttClientService : IHostedService,
                                          IMqttClientConnectedHandler,
                                          IMqttClientDisconnectedHandler,
                                          IMqttApplicationMessageReceivedHandler
    {
        Task PublishMessageAsync(string topic, string payload);
    }
}
