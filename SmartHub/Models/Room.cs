using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models
{
    public class Room
    {
        public long RoomID { get; set; }
        public string RoomName { get; set; }

        //Navigation Properties
        public ICollection<Gateway> Gateways { get; set; }
    }
}
