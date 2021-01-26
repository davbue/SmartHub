using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class Device
    {
        public long DeviceID { get; set; }
        public string DeviceName { get; set; }
        public bool Enabled { get; set; }
        public long GatewayID { get; set; }
        public long DeviceTypeID { get; set; }

        //Navigation Properties
        public Gateway Gateway { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
