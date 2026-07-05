using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Builders;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Conversations.Commands
{
    public record SendMessageCommand(Guid ConversationId, string Content) : IRequest<SendMessageResult>;

    public record SendMessageResult(bool Success, string? AiResponse, string? ErrorMessage);

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IJournalRepository _journalRepository;
        private readonly IAiService _aiService;
        private readonly ICurrentUser _currentUser;

        public SendMessageCommandHandler(IConversationRepository conversationRepository, IJournalRepository journalRepository, IAiService aiService, ICurrentUser currentUser)
        {
            _conversationRepository = conversationRepository;
            _journalRepository = journalRepository;
            _aiService = aiService;
            _currentUser = currentUser;
        }

        public async Task<SendMessageResult> Handle(
    SendMessageCommand request, CancellationToken ct)
        {
            // 1. Lấy conversation và validate
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, ct);

            if (conversation is null || conversation.UserId != _currentUser.Id)
                return new SendMessageResult(false, null,
                    "Không tìm thấy cuộc trò chuyện.");

            // 2. Lấy toàn bộ nhật ký làm context cho AI
            var journals = await _journalRepository
                .GetByUserIdAsync(_currentUser.Id, ct);

            var journalContext = journals
                .Where(j => j.HasContent())
                .OrderBy(j => j.CreatedAt)
                .Select(j => $"[{j.CreatedAt:dd/MM/yyyy}]: {j.Content}")
                .ToList();

            // 3. Builder xây dựng system prompt
            var systemPrompt = new AiPromptBuilder()
                .WithBaseInstruction(
                    """
                    Bạn đang đóng vai "phiên bản quá khứ" của người dùng này.
                    Dưới đây là toàn bộ nhật ký họ đã viết theo thời gian.
                    Hãy trả lời như thể bạn chính là họ — dùng ngôi thứ nhất,
                    chân thật với cảm xúc và suy nghĩ được ghi lại trong nhật ký.
                    QUAN TRỌNG: Chỉ trả lời bằng tiếng Việt, không dùng ngôn ngữ khác.
                    """)
                .WithJournalContext(journalContext)
                .BuildSystemPrompt();

            // 4. Lưu tin nhắn user — thêm trực tiếp vào DbContext
            var userMessage = Message.CreateUserMessage(
                request.ConversationId,
                request.Content);

            await _conversationRepository.AddMessageAsync(userMessage, ct);

            // 5. Gọi AI
            var aiResponse = await _aiService.ChatWithPastSelfAsync(
                request.Content,
                journalContext,
                ct);

            // 6. Lưu response AI — thêm trực tiếp vào DbContext
            var assistantMessage = Message.CreateAssistantMessage(
                request.ConversationId,
                aiResponse);

            await _conversationRepository.AddMessageAsync(assistantMessage, ct);

            // 7. Update LastMessageTime của conversation
            conversation.UpdateLastMessageTime();
            await _conversationRepository.UpdateAsync(conversation, ct);

            // 8. Một lần SaveChanges duy nhất — Unit of Work pattern
            await _conversationRepository.SaveChangesAsync(ct);

            return new SendMessageResult(true, aiResponse, null);
        }
    }
}
