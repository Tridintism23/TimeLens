using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Journals.Commands
{
    public record CreateJournalEntryCommand (string? Title, string? Content) : IRequest<Guid>;
    public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, Guid>
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICurrentUser _currentUser;

        public CreateJournalEntryCommandHandler (IJournalRepository journalRepository, ICurrentUser currentUser)
        {
            _journalRepository = journalRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateJournalEntryCommand request, CancellationToken ct)
        {
            var entry = JournalEntry.Create(
                _currentUser.Id, 
                request.Title, 
                request.Content);

            await _journalRepository.AddAsync(entry, ct);
            await _journalRepository.SaveChangesAsync(ct);

            return entry.Id;
        }
    }
}
