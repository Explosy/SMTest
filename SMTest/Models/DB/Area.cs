using System.Collections.Generic;

#nullable disable

namespace SMTest.Models.DB
{
    public partial class Area
    {
        public Area()
        {
            BusyPickets = new HashSet<BusyPicket>();
        }

        public int AreaId { get; set; }
        public string Title { get; set; }
        public int WareHouseId { get; set; }

        public virtual WareHouse WareHouse { get; set; }
        public virtual ICollection<BusyPicket> BusyPickets { get; set; }
    }
}
