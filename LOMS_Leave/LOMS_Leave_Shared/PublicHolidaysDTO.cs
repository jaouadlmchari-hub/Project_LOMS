using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Shared
{
    public class PublicHolidaysDTO
    {
        public int HolidayID { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
        public bool IsRepeatedAnnually { get; set; }
    }
}
