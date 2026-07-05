using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Application.Common.Builders;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Reflections
{
    public class ReflectionEngine
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IReflectionRepository _reflectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAiService _aiService;
        private readonly PhilosophyStrategyFactory _strategyFactory;

        public ReflectionEngine(IJournalRepository journalRepository, IReflectionRepository reflectionRepository, IUserRepository userRepository, IAiService aiService, PhilosophyStrategyFactory strategyFactory)
        {
            _journalRepository = journalRepository;
            _reflectionRepository = reflectionRepository;
            _userRepository = userRepository;
            _aiService = aiService;
            _strategyFactory = strategyFactory;
        }

        public async Task<ReflectionEngineResult> GenerateAsync(Guid journalEntryId, Guid userId, CancellationToken ct = default)
        {
            var entry = await _journalRepository.GetByIdAsync(journalEntryId, ct);

            if (entry is null || entry.UserId != userId) 
            {
                return ReflectionEngineResult.Fail("Không tìm thấy nhật ký.");
            }

            if (!entry.HasContent())
            {
                return ReflectionEngineResult.Fail("Nhật ký chưa có nội dung để generate reflection.");
            }

            var user = await _userRepository.GetByIdAsync(userId, ct);
            var philosophy = user?.PreferredPhilosophy ?? Domain.Entities.PhilosophyType.Stoicism;

            var strategy = _strategyFactory.GetStrategy(philosophy);

            var systemPrompt = new AiPromptBuilder().WithPhilosophy(strategy.BuildSystemPrompt(entry.Content)).BuildSystemPrompt();

            var existing = await _reflectionRepository.GetActiveByJournalEntryIdAsync(journalEntryId, ct);

            if (existing is not null)
            {
                existing.Deactivate();
                await _reflectionRepository.UpdateAsync(existing, ct);
            }

            var questions = await _aiService.GenerateReflectionQuestionsAsync(entry.Content, systemPrompt, ct);

            var lastestVersion = await _reflectionRepository.GetLatestVersionAsync(journalEntryId, ct);

            var reflection = ReflectionEntry.Create(journalEntryId, philosophy, questions, lastestVersion + 1);

            await _reflectionRepository.AddAsync(reflection, ct);
            await _reflectionRepository.SaveChangesAsync(ct);

            return ReflectionEngineResult.Ok(questions, philosophy.ToString()); 
        }

        public async Task<string> GenerateWeeklyInsightAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var journals = await _journalRepository.GetByDateRangeAsync(userId, from, to, ct);

            var moodEntries = await GetMoodRepositoryAsync(userId, from, to, ct);

            if (!journals.Any()) 
            {
                return "Chưa có đủ dữ liệu nhật ký trong tuần này.";
            }

            var systemPrompt = new AiPromptBuilder().WithBaseInstruction("Bạn là một người bạn thông thái đang giúp người dùng nhìn lại tuần vừa qua.").WithJournalContext(journals.Select(j => j.Content)).WithMoodContext((int)moodEntries.Average()).BuildSystemPrompt();

            return await _aiService.GenerateWeeklyInsightAsync(journals.Select(j => j.Content), moodEntries, ct);
        }

        private Task<List<int>> GetMoodRepositoryAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
            => Task.FromResult(new List<int>());

        public record ReflectionEngineResult(bool Success, string? Questions, string? Philosophy, string? ErrorMessage)
        {
            public static ReflectionEngineResult Ok(string questions, string philosophy)
                => new(true, questions, philosophy, null);

            public static ReflectionEngineResult Fail(string error)
                => new(false, null, null, error);
        }
    }
}
