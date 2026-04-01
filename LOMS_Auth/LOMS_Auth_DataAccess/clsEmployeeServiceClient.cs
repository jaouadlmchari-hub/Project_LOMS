using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LOMS_Auth_DataAccess
{
    namespace LOMS_Auth_DataAccess
    {
        public class clsEmployeeServiceClient
        {
            private static readonly HttpClient _httpClient = new HttpClient();

            // Appelle l'API du microservice Employee
            public static async Task<dynamic> GetEmployeeBasicInfoAsync(int EmployeeID)
            {
                string url = $"https://localhost:7175/api/employees/{EmployeeID}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<dynamic>(json);
                }
                return null;
            }
        }
    }
}
