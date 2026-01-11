using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Respace
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}