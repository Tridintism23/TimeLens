using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using TimeLens.Application.Common.Interfaces;

namespace TimeLens.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public AiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var apiKey = configuration["Groq:ApiKey"]!;
            _model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";

            _httpClient = httpClientFactory.CreateClient("Groq");
            _httpClient.BaseAddress = new Uri("https://api.groq.com");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        private async Task<string> CallGroqAsync(
            string systemPrompt,
            string userMessage,
            int maxTokens = 1000,
            CancellationToken ct = default)
        {
            var body = new
            {
                model = _model,
                messages = new[]
                {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
                max_tokens = maxTokens,
                temperature = 0.7
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/openai/v1/chat/completions", body, ct);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"Groq 429 detail: {errorBody}");
                throw new InvalidOperationException(
                    "Rate limit exceeded. Vui lòng thử lại sau.");
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }

        public async Task<string> GenerateReflectionQuestionsAsync(
            string journalContent,
            string systemPrompt,
            CancellationToken ct = default)
        {
            return await CallGroqAsync(
                systemPrompt,
                $"Nhật ký của tôi hôm nay:\n{journalContent}",
                ct: ct);
        }

        public async Task<string> GenerateTitleAsync(
            string journalContent,
            CancellationToken ct = default)
        {
            return await CallGroqAsync(
                "Tạo một tiêu đề ngắn gọn (tối đa 10 từ) cho nhật ký sau. Chỉ trả về tiêu đề, không có gì khác.",
                journalContent,
                maxTokens: 100,
                ct: ct);
        }

        public async Task<(int Score, string Reasoning)> AnalyzeMoodAsync(
            string journalContent,
            CancellationToken ct = default)
        {
            var result = await CallGroqAsync(
                """
            Phân tích cảm xúc trong nhật ký và trả về JSON:
            {"score": <1-5>, "reasoning": "<lý do ngắn gọn>"}
            1=rất tiêu cực, 3=bình thường, 5=rất tích cực.
            Chỉ trả về JSON, không có gì khác.
            """,
                journalContent,
                maxTokens: 200,
                ct: ct);

            try
            {
                var clean = result.Replace("```json", "").Replace("```", "").Trim();
                using var doc = JsonDocument.Parse(clean);
                var score = doc.RootElement.GetProperty("score").GetInt32();
                var reasoning = doc.RootElement.GetProperty("reasoning").GetString() ?? "";
                return (score, reasoning);
            }
            catch
            {
                return (3, "Không phân tích được");
            }
        }

        public async Task<string> ChatWithPastSelfAsync(
            string userMessage,
            IEnumerable<string> journalContext,
            CancellationToken ct = default)
        {
            var contextText = string.Join("\n---\n", journalContext);

            return await CallGroqAsync(
                $"""
            Bạn đang đóng vai "phiên bản quá khứ" của người dùng.
            Đây là nhật ký họ đã viết — suy nghĩ và cảm xúc thực của họ.
            Hãy trả lời như thể bạn chính là họ trong quá khứ, dùng ngôi thứ nhất.

            Nhật ký quá khứ:
            {contextText}
            """,
                userMessage,
                ct: ct);
        }

        public async Task<string> GenerateWeeklyInsightAsync(
            IEnumerable<string> journalContext,
            IEnumerable<int> moodScores,
            CancellationToken ct = default)
        {
            var contextText = string.Join("\n---\n", journalContext);
            var scores = moodScores.ToList();
            var avgMood = scores.Any() ? scores.Average().ToString("F1") : "N/A";

            return await CallGroqAsync(
                $"""
            Tổng kết tuần dựa trên nhật ký và mood của người dùng.
            Mood trung bình: {avgMood}/5.
            Viết 3-4 đoạn ngắn, chân thật, có chiều sâu.
            """,
                $"Nhật ký tuần này:\n{contextText}",
                ct: ct);
        }
    }
}
