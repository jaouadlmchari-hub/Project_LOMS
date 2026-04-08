using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Auth_Shared
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        public int EmployeeID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<int> SelectedRolesIDs { get; set; } = new List<int>();

    }
}
