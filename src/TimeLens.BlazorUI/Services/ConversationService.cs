using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace TimeLens.BlazorUI.Services
{
    public record ConversationDto(
        Guid Id,
        string Title,
        DateTime CreatedAt,
        DateTime LastMessageAt);

    public record MessageDto(
        Guid Id,
        string Role,
        string Content,
        DateTime CreatedAt);
    public class ConversationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public ConversationService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private void EnsureAuthHeader()
        {
            if (AuthService.CurrentToken is not null)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", AuthService.CurrentToken);
            }
        }

        public async Task<List<ConversationDto>> GetAllAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<ConversationDto>>("api/conversations");

            return result ?? new List<ConversationDto>();
        }

        public async Task<Guid?> StartAsync(string? title = null)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/conversations", new { title });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("id").GetGuid();
        }

        public async Task<string?> SendMessageAsync(Guid conversationId, string content)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/conversations/{conversationId}/messages",
                JsonSerializer.Serialize(content));

            if (!response.IsSuccessStatusCode) {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("response").GetString();
        }
    }
}
