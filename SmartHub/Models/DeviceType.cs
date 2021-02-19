using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class DeviceType
    {
        public long DeviceTypeId { get; set; }
        public string DeviceTypeName { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public bool Sensor { get; set; }

        //Navigation Properties
        public ICollection<Device> Devices { get; set; }
    }
}
