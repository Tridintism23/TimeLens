using System.Net.Http.Json;
using System.Text.Json;

namespace TimeLens.BlazorUI.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    // Static để tất cả services đều đọc được cùng token
    public static string? CurrentToken { get; private set; }

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(CurrentToken);
    public string? Token => CurrentToken;

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        CurrentToken = result.GetProperty("token").GetString();
        SetAuthHeader();
        return true;
    }

    public async Task<bool> RegisterAsync(
        string email, string fullName, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
        {
            email,
            fullName,
            password
        });

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        CurrentToken = result.GetProperty("token").GetString();
        SetAuthHeader();
        return true;
    }

    public void Logout()
    {
        CurrentToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private void SetAuthHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", CurrentToken);
    }
}