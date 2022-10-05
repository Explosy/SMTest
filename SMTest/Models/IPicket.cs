using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTest.Models
{
    interface IPicket
    {
        int PicketId { get; set; }
        int PicketNumber { get; set; }
        int Cargo { get; set; }
        DateTime DateStart { get; set; }
    }
}
