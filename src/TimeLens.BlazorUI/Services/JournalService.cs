using System.Net.Http.Json;
using System.Text.Json;

namespace TimeLens.BlazorUI.Services
{
    public record JournalEntryDto(
        Guid Id,
        string Title,
        string Content,
        bool IsTitleAiGenerated,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
    public class JournalService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public JournalService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        // Gọi trước mỗi request để đảm bảo token luôn được set
        private void EnsureAuthHeader()
        {
            if (AuthService.CurrentToken is not null)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", AuthService.CurrentToken);
            }
        }

        public async Task<List<JournalEntryDto>> GetAllAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<JournalEntryDto>>("api/journals");

            return result ?? new List<JournalEntryDto>();
        }

        public async Task<Guid?> CreateAsync(string? title, string? content)
        {
            var response = await _httpClient.PostAsJsonAsync("api/journals", new
            {
                title,
                content
            });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("id").GetGuid();
        }

        public async Task<bool> UpdateAsync(Guid id, string? title, string? content)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/journals/{id}", new
            {
                id,
                title,
                content
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<string?> GenerateReflectionAsync(Guid journalEntryId)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/reflections/generate", new { journalEntryId });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("questions").GetString();
        }
    }
}
