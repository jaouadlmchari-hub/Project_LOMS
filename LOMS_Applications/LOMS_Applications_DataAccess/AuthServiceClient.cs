using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LOMS_Applications_DataAccess
{
    public class AuthServiceClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public AuthServiceClient()
        {
            // On récupère l'URL du conteneur Auth (Port 8081 en interne Docker)
            string baseUrl = Environment.GetEnvironmentVariable("AUTH_API_URL") ?? "http://auth-container:8080/";
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
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