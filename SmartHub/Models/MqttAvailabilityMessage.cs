using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class MqttAvailabilityMessage
    {
        public string GatewayId { get; set; }
        public bool Online { get; set; }

    }
}
