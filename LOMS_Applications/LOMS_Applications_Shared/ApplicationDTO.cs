using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Applications_Shared
{
    public class ApplicationDTO
    {
        public int ApplicationID {  get; set; }
        public int EmployeeID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
        public DateTime LastStatusDate { get; set; }
        public int CreatedByUserID { get; set; }
        public int ApplicationTypeID { get; set; }
        public string Notes { get; set; }
        public int ActionByUserID { get; set; } = -1;

    }
}
