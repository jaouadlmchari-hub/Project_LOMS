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

            public static string BaseUrl { get; set; } = "http://localhost:7175";
            public static async Task<EmployeeBasicInfoDTO?> GetEmployeeBasicInfoAsync(int EmployeeID)
            {
                // On garde cette ligne pour Docker si  n'as pas de certificat valide

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string url = $"{BaseUrl.TrimEnd('/')}/api/employees/{EmployeeID}";
                try
                {
                    var response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // On force Newtonsoft à être tolérant sur la casse (Majuscules/Minuscules)
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };

                        return JsonConvert.DeserializeObject<EmployeeBasicInfoDTO>(json, settings);
                    }
                }
                catch (Exception ex)
                {
                    // Debug : voir l'erreur exacte dans les logs Docker
                    Console.WriteLine($"[API Error]: {ex.Message}");
                }

                return null;
            }
        }
    }
}
