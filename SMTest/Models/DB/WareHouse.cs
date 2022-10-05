using System;
using System.Collections.Generic;

#nullable disable

namespace SMTest.Models.DB
{
    public partial class WareHouse
    {
        public WareHouse()
        {
            Areas = new HashSet<Area>();
            FreePickets = new HashSet<FreePicket>();
        }

        public int WareHouseId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Area> Areas { get; set; }
        public virtual ICollection<FreePicket> FreePickets { get; set; }
    }
}
