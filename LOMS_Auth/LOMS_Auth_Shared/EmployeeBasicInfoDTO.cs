using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOMS_Auth_Shared
{
    public class EmployeeBasicInfoDTO
    {
        [JsonProperty("employeeID")] // Force le lien même si le JSON est en minuscule
        public int EmployeeID { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}
