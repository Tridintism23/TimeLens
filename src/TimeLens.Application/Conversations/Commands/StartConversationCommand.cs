using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Conversations.Commands
{
    public record StartConversationCommand(string? Title) : IRequest<Guid>;

    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, Guid>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ICurrentUser _currentUser;

        public StartConversationCommandHandler(IConversationRepository conversationRepository, ICurrentUser currentUser)
        {
            _conversationRepository = conversationRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(StartConversationCommand request, CancellationToken ct)
        {
            var conversation = Conversation.Create(_currentUser.Id, request.Title);

            await _conversationRepository.AddAsync(conversation, ct);
            await _conversationRepository.SaveChangesAsync(ct);

            return conversation.Id;
        }
    }
}
