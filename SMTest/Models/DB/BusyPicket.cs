using System;
using System.Collections.Generic;

#nullable disable

namespace SMTest.Models.DB
{
    public partial class BusyPicket : IPicket
    {
        public int PicketId { get; set; }
        public int PicketNumber { get; set; }
        public int AreaId { get; set; }
        public int Cargo { get; set; }
        public DateTime DateStart { get; set; }

        public virtual Area Area { get; set; }
    }
}
