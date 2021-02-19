using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class DataEntity
    {
        public string DeviceId { get; set; }
        public float Value { get; set; }
        public long DeviceTypeId { get; set; }
        public bool Enabled { get; set; }
    }
}
