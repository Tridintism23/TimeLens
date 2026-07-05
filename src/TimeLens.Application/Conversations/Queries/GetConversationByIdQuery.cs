using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Conversations.Queries
{
    public record GetConversationByIdQuery(Guid Id) : IRequest<Conversation?>;

    public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, Conversation?>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ICurrentUser _currentUser;

        public GetConversationByIdQueryHandler(IConversationRepository conversationRepository, ICurrentUser currentUser)
        {
            _conversationRepository = conversationRepository;
            _currentUser = currentUser;
        }

        public async Task<Conversation?> Handle(GetConversationByIdQuery request, CancellationToken ct)
        {
            var conversation = await _conversationRepository
                .GetByIdWithMessagesAsync(request.Id, ct);

            if (conversation is null || conversation.UserId != _currentUser.Id)
            {
                return null;
            }
            return conversation;
        }
    }
}
