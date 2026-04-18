public class clsLeaveServiceClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static string? _baseUrl;

    public static void Initialize(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public static async Task<bool> InitLeaveBalanceAsync(int employeeId)
    {
        if (string.IsNullOrEmpty(_baseUrl)) return false;

        try
        {
            // L'URL devient : http://leave-container:8083/api/LeaveBalances/init/{id}
            string url = $"{_baseUrl}/init/{employeeId}";
            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LeaveServiceClient Error]: {ex.Message}");
            return false;
        }
    }
}