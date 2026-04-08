using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Shared
{
    public class LeaveBalanceDTO
    {
        public int LeaveBalanceID { get; set; }
        public int EmployeeID { get; set; }
        public int LeaveTypeID { get; set; }
        public int Year { get; set; }
        public decimal EntitledDays { get; set; }
        public decimal UsedDays { get; set; }
        public decimal RemainingDays { get; set; }
    }
}
