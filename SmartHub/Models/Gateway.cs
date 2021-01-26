using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class Gateway
    {
        public long GatewayID { get; set; }
        public long RoomID { get; set; }

        //Navigation Properties
        public ICollection<Device> Devices { get; set; }
        public Room Room { get; set; }
    }
}
