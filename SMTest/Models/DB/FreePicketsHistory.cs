using System;
using System.Collections.Generic;

#nullable disable

namespace SMTest.Models.DB
{
    public partial class FreePicketsHistory : IPicket
    {
        public int PicketId { get; set; }
        public int PicketNumber { get; set; }
        public int WareHouseId { get; set; }
        public int Cargo { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
