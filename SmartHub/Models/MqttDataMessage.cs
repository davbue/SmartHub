using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class MqttDataMessage
    {
        public string GatewayId { get; set; }
        public long Timestamp { get; set; }
        public List<DataEntity> DataEntities { get; set; }

    }
}
