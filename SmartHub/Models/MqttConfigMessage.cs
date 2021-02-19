using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class MqttConfigMessage
    {
        public string GatewayId { get; set; }
        public long Timestamp { get; set; }
        public List<DeviceConfig> DeviceConfigs { get; set; }

    }
}
