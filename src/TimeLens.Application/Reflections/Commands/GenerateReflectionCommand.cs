using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Application.Reflections;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Reflections.Commands
{
    public record GenerateReflectionCommand(Guid JournalEntryId) : IRequest<GenerateReflectionResult>;

    public record GenerateReflectionResult(bool Success, string? Questions, string? ErrorMessage);

    public class GenerateReflectionCommandHandler : IRequestHandler<GenerateReflectionCommand, GenerateReflectionResult>
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IReflectionRepository _reflectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAiService _aiService;
        private readonly PhilosophyStrategyFactory _strategyFactory;
        private readonly ICurrentUser _currentUser;

        public GenerateReflectionCommandHandler(IJournalRepository journalRepository, IReflectionRepository reflectionRepository, IUserRepository userRepository, IAiService aiService, PhilosophyStrategyFactory strategyFactory, ICurrentUser currentUser)
        {
            _journalRepository = journalRepository;
            _reflectionRepository = reflectionRepository;
            _userRepository = userRepository;
            _aiService = aiService;
            _strategyFactory = strategyFactory;
            _currentUser = currentUser;
        }

        public async Task<GenerateReflectionResult> Handle(GenerateReflectionCommand request, CancellationToken ct)
        {
            // 1. Lấy journal entry
            var entry = await _journalRepository.GetByIdAsync(request.JournalEntryId, ct);

            if (entry is null || entry.UserId != _currentUser.Id)
                return new GenerateReflectionResult(false, null, "Không tìm thấy nhật ký.");

            if (!entry.HasContent())
                return new GenerateReflectionResult(false, null,
                    "Nhật ký chưa có nội dung để generate reflection.");

            // 2. Lấy preferred philosophy của user
            var user = await _userRepository.GetByIdAsync(_currentUser.Id, ct);
            var philosophy = user?.PreferredPhilosophy ?? PhilosophyType.Stoicism;

            // 3. Factory lấy đúng Strategy — Strategy build prompt
            var strategy = _strategyFactory.GetStrategy(philosophy);
            var systemPrompt = strategy.BuildSystemPrompt(entry.Content);

            // 4. Deactivate reflection cũ nếu có (chuẩn bị regenerate)
            var existingReflection = await _reflectionRepository
                .GetActiveByJournalEntryIdAsync(request.JournalEntryId, ct);

            if (existingReflection is not null)
            {
                existingReflection.Deactivate();
                await _reflectionRepository.UpdateAsync(existingReflection, ct);
            }

            // 5. Gọi AI — AiService chỉ nhận prompt đã build sẵn
            var questions = await _aiService.GenerateReflectionQuestionsAsync(
                entry.Content, systemPrompt, ct);

            // 6. Tính version mới
            var latestVersion = await _reflectionRepository
                .GetLatestVersionAsync(request.JournalEntryId, ct);

            // 7. Tạo ReflectionEntry mới — Factory Method pattern
            var reflection = ReflectionEntry.Create(
                request.JournalEntryId,
                philosophy,
                questions,
                latestVersion + 1);

            await _reflectionRepository.AddAsync(reflection, ct);
            await _reflectionRepository.SaveChangesAsync(ct);

            return new GenerateReflectionResult(true, questions, null);
        }
    }
}
