using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TimeLens.Application.Common.Interfaces;

// Decorator pattern — bọc AiService thật với cache
// _inner = AiService thật, _cache = IMemoryCache
public class CachedAiService : IAiService
{
    private readonly IAiService _inner;        // AiService thật bên trong
    private readonly IMemoryCache _cache;      // Cache layer bên ngoài
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public CachedAiService(IAiService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<string> GenerateReflectionQuestionsAsync(
        string journalContent,
        string systemPrompt,
        CancellationToken ct = default)
    {
        var cacheKey = $"reflection_{systemPrompt.GetHashCode()}_{journalContent.GetHashCode()}";

        if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
            return cached;

        var result = await _inner.GenerateReflectionQuestionsAsync(
            journalContent, systemPrompt, ct);

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<string> GenerateTitleAsync(
        string journalContent,
        CancellationToken ct = default)
    {
        var cacheKey = $"title_{journalContent.GetHashCode()}";

        if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
            return cached;

        var result = await _inner.GenerateTitleAsync(journalContent, ct);
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    // Mood và Chat không cache — cần fresh data mỗi lần gọi
    public Task<(int Score, string Reasoning)> AnalyzeMoodAsync(
        string journalContent,
        CancellationToken ct = default)
        => _inner.AnalyzeMoodAsync(journalContent, ct);

    public Task<string> ChatWithPastSelfAsync(
        string userMessage,
        IEnumerable<string> journalContext,
        CancellationToken ct = default)
        => _inner.ChatWithPastSelfAsync(userMessage, journalContext, ct);

    public Task<string> GenerateWeeklyInsightAsync(
        IEnumerable<string> journalContext,
        IEnumerable<int> moodScores,
        CancellationToken ct = default)
        => _inner.GenerateWeeklyInsightAsync(journalContext, moodScores, ct);
}
