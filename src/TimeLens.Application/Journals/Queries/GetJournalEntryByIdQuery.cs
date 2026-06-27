using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Journals.Queries
{
    public record GetJournalEntryByIdQuery(Guid id) : IRequest<JournalEntry?>;

    public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, JournalEntry?>
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICurrentUser _currentUser;

        public GetJournalEntryByIdQueryHandler(IJournalRepository journalRepository, ICurrentUser currentUser)
        {
            _journalRepository = journalRepository;
            _currentUser = currentUser;
        }
        public async Task<JournalEntry?> Handle(GetJournalEntryByIdQuery request, CancellationToken ct)
        {
            var entry = await _journalRepository.GetByIdAsync(request.id, ct);

            if(entry is null || entry.UserId != _currentUser.Id)
            {
                return null;
            }

            return entry;
        }
    }
}
