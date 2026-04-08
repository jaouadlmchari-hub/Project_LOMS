using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Applications_DataAccess
{
    public class EmployeeServiceClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static string BaseUrl { get; set; }

        public EmployeeServiceClient()
        {
            // On configure l'adresse de base une seule fois
            if (_httpClient.BaseAddress == null && !string.IsNullOrEmpty(BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(BaseUrl.EndsWith("/") ? BaseUrl : BaseUrl + "/");
            }
        }

        public async Task<string> GetEmployeeFullName(int employeeID)
        {
            try
            {
                // Assure-toi que le endpoint existe dans ton service LOMS_Employee
                var response = await _httpClient.GetAsync($"api/employees/{employeeID}/fullname");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
                // Log l'erreur ici si besoin
            }
            return "Unknown Employee";
        }
    }
}