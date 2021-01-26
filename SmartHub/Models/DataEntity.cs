using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class DataEntity
    {
        public long GatewayID { get; set; }
        public long DeviceID { get; set; }
        public int Value { get; set; }
        public string Unit { get; set; }
        public bool Enabled { get; set; }
    }
}
