using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Journals.Commands
{
    public record UpdateJournalEntryCommand(Guid Id, string? Title, string? Content) : IRequest<bool>;

    public class UpdateJournalEntryCommandHandler : IRequestHandler<UpdateJournalEntryCommand, bool>
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICurrentUser _currentUser;

        public UpdateJournalEntryCommandHandler(IJournalRepository journalRepository, ICurrentUser currentUser)
        {
            _journalRepository = journalRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdateJournalEntryCommand request, CancellationToken ct)
        {
            var entry = await _journalRepository.GetByIdAsync(request.Id, ct);

            if (entry is null || entry.UserId != _currentUser.Id)
            {
                return false;
            }

            entry.Update(request.Title, request.Content);

            await _journalRepository.UpdateAsync(entry, ct);
            await _journalRepository.SaveChangesAsync(ct);

            return true;
        }
    }
}
