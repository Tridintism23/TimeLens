using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Journals.Queries
{
    public record GetJournalEntriesQuery : IRequest<List<JournalEntry>>;

    public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, List<JournalEntry>>
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICurrentUser _currentUser;

        public GetJournalEntriesQueryHandler(IJournalRepository journalRepository, ICurrentUser currentUser)
        {
            _journalRepository = journalRepository;
            _currentUser = currentUser;
        }

        public async Task<List<JournalEntry>> Handle(GetJournalEntriesQuery request, CancellationToken ct)
        {
            return await _journalRepository.GetByUserIdAsync(_currentUser.Id, ct);
        }
    }
}
