using LOMS_Employee_Shared;
using Newtonsoft.Json;
using System.Text;

public class clsSalaryServiceClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static string? _baseUrl;

    public static void Initialize(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public static async Task<decimal> GetSalaryByEmployeeIDAsync(int employeeID)
    {
        if (string.IsNullOrEmpty(_baseUrl)) return 0;

        try
        {
            string url = $"{_baseUrl}/by-employee/{employeeID}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Désérialisation vers le DTO local
                var result = JsonConvert.DeserializeObject<SalaryResponseDTO>(json);

                return result?.Salary ?? 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SalaryServiceClient Error]: {ex.Message}");
        }
        return 0;
    }

    public static async Task<decimal> GetSalaryByNationalNoAsync(string nationalNo)
    {
        if (string.IsNullOrEmpty(_baseUrl)) return 0;

        try
        {
            // On appelle la route que nous avons créée dans le SalaryController
            string url = $"{_baseUrl}/by-national-no/{nationalNo}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // On utilise le même DTO local
                var result = JsonConvert.DeserializeObject<SalaryResponseDTO>(json);

                return result?.Salary ?? 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SalaryServiceClient Error - NationalNo]: {ex.Message}");
        }
        return 0;
    }

    public static async Task<bool> CreateInitialSalaryAsync(int employeeID, decimal amount, int createdBy)
    {
        try
        {
            var salaryData = new
            {
                EmployeeID = employeeID,
                Salary = amount,
                CreatedByUserID = createdBy,
                EffectiveDate = DateTime.Now
            };

            string json = JsonConvert.SerializeObject(salaryData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/add", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}