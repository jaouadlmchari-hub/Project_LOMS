using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LOMS_Applications_DataAccess
{
    public class AuthServiceClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static string baseUrl = Environment.GetEnvironmentVariable("AUTH_API_URL") ?? "http://auth-container:8081/";
        public AuthServiceClient()
        {

            if (_httpClient.BaseAddress == null)
            {
                // On s'assure que l'URL finit par un '/' pour HttpClient
                _httpClient.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            }
        }

        public async Task<string> GetUserName(int userID)
        {
            try
            {
                // On appelle l'endpoint que tu vas créer dans LOMS_Auth
                var response = await _httpClient.GetAsync($"api/users/{userID}/username");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                // Optionnel : Log l'erreur ex.Message pour le debug Docker
            }
            return "User Not Found";
        }
    }
}