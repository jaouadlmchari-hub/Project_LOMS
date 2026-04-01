using Newtonsoft.Json; 
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class clsSalaryServiceClient
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<decimal> GetSalaryByEmployeeIDAsync(int employeeID)
    {
        try
        {
            // Remplace le port par celui de ton futur microservice Salary
            string url = $"https://localhost:7200/api/salaries/{employeeID}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // On suppose que l'API renvoie juste le montant ou un objet SalaryDTO
                return JsonConvert.DeserializeObject<decimal>(json);
            }
        }
        catch
        {
            
        }
        return 0; 
    }
}