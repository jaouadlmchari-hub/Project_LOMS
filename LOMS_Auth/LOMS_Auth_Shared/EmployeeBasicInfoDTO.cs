using System;
using System.Collections.Generic;
using System.Text;

namespace LOMS_Auth_Shared
{
    public class EmployeeBasicInfoDTO
    {
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? NationalNo { get; set; }
    }
}
