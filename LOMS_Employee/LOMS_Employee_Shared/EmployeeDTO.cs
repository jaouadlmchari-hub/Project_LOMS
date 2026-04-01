using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Employee_Shared
{
 
    public class EmployeeDTO
    {
        public int EmployeeID { get; set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public short Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public int NationalityCountryID { get; set; }
        public string ImagePath { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DepartmentID { get; set; }
        public int JobTitleID { get; set; }
        public int CreatedByUserID { get; set; }  


    }

}
