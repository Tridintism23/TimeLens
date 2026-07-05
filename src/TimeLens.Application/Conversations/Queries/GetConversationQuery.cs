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
    public record GetConversationQuery : IRequest<List<Conversation>>;

    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, List<Conversation>>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ICurrentUser _currentUser;

        public GetConversationQueryHandler(IConversationRepository conversationRepository, ICurrentUser currentUser)
        {
            _conversationRepository = conversationRepository;
            _currentUser = currentUser;
        }

        public async Task<List<Conversation>> Handle(GetConversationQuery request, CancellationToken ct)
        {
            return await _conversationRepository.GetByUserIdAsync(_currentUser.Id, ct);
        }
    }
}
