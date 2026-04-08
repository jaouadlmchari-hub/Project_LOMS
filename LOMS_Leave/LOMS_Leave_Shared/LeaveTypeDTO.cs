using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Shared
{
    public class LeaveTypeDTO
    {
        public int LeaveTypeID { get; set; }
        public string LeaveName { get; set; }
        public bool IsPaid { get; set; }
        public short MaxDaysPerYear { get; set; }
        public bool RequiresDocument {  get; set; }
        public bool IsActive { get; set; }

    }
}
