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

        public EmployeeServiceClient()
        {
            // On change localhost par le nom du conteneur pour que ça marche dans Docker par défaut
            string baseUrl = Environment.GetEnvironmentVariable("EMPLOYEE_API_URL") ?? "http://employee-container:8080/";

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
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