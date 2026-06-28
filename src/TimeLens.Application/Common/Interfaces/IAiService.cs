using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Common.Interfaces
{
    public interface IAiService
    {
        Task<string> GenerateReflectionQuestionsAsync(string journalContent, string systemPrompt, CancellationToken ct = default);

        Task<string> GenerateTitleAsync(string journalContent, CancellationToken ct = default);

        Task<(int Score, string Reasoning)> AnalyzeMoodAsync(string journalContent, CancellationToken ct = default);

        Task<string> ChatWithPastSelfAsync(string userMessage, IEnumerable<string> journalContext, CancellationToken ct = default);

        Task<string> GenerateWeeklyInsightAsync(IEnumerable<string> journalContext, IEnumerable<int> moodScores, CancellationToken ct = default);
    }
}
