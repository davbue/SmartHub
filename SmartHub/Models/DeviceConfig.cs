using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class DeviceConfig
    {
        public string DeviceId { get; set; }
        public long DeviceTypeId { get; set; }
        public List<int> Pins { get; set; }
        public bool Delete { get; set; }
        public bool Enabled { get; set; }

    }
}
