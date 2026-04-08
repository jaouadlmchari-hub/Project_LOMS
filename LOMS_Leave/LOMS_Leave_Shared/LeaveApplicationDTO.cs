using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Shared
{
    public class LeaveApplicationDTO
    {
        public int LeaveApplicationID { get; set; }
        public int ApplicationID { get; set; }
        public int LeaveTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal NumberOfDays { get; set; }
        public string Reason { get; set; }

    }
}
