using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Salary_Shared
{
    public class EmployeeSalaryDTO
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}