using LOMS_Auth_Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                //string url = $"https://localhost:7175/api/employees/{EmployeeID}";
                //string url = $"https://host.docker.internal:7175/api/employees/{EmployeeID}";
                string url = $"http://employee-container:8080/api/employees/{EmployeeID}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmployeeBasicInfoDTO>(json);
                }
                return null;
            }
        }
    }
}
