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

        public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken ct)
        {
            var conversation = await _conversationRepository.GetByIdWithMessagesAsync(request.ConversationId, ct);

            if (conversation is null || conversation.UserId != _currentUser.Id)
            {
                return new SendMessageResult(false, null, "Không tìm thấy cuộc trò chuyện.");
            }

            var journals = await _journalRepository.GetByUserIdAsync(_currentUser.Id, ct);

            var journalContext = journals
                .Where(j => j.HasContent())
                .OrderBy(j => j.CreatedAt)
                .Select(j => $"[{j.CreatedAt:dd/MM/yyyy}]: {j.Content}")
                .ToList();

            var systemPrompt = new AiPromptBuilder().WithBaseInstruction(
                """
                Bạn đang đóng vai "phiên bản quá khứ" của người dùng này.
                Dưới đây là toàn bộ nhật ký họ đã viết theo thời gian.
                Hãy trả lời như thể bạn chính là họ — dùng ngôi thứ nhất,
                chân thật với cảm xúc và suy nghĩ được ghi lại trong nhật ký.
                Nếu câu hỏi liên quan đến thời điểm cụ thể, hãy tham chiếu
                đúng nội dung nhật ký của ngày đó.
                """)
                .WithJournalContext(journalContext)
                .BuildSystemPrompt();

            var userMessage = Message.CreateUserMessage(request.ConversationId, request.Content);

            conversation.Messages.Add(userMessage);
            conversation.UpdateLastMessageTime();

            var aiResponse = await _aiService.ChatWithPastSelfAsync(
                request.Content, journalContext, ct);

            var assistantMessage = Message.CreateAssistantMessage(request.ConversationId, aiResponse);

            conversation.Messages.Add(assistantMessage);

            await _conversationRepository.UpdateAsync(conversation, ct);
            await _conversationRepository.SaveChangesAsync(ct);

            return new SendMessageResult(true, aiResponse, null);
        }
    }
}
