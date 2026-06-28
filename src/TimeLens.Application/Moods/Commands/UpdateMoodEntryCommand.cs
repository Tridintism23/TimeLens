using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Moods.Commands
{
    public record UpdateMoodEntryCommand(Guid Id, int Score, string? Note) : IRequest<bool>;

    public class UpdateMoodEntryCommandHandler : IRequestHandler<UpdateMoodEntryCommand, bool>
    {
        private readonly IMoodRepository _moodRepository;
        private readonly ICurrentUser _currentUser;

        public UpdateMoodEntryCommandHandler(IMoodRepository moodRepository, ICurrentUser currentUser)
        {
            _moodRepository = moodRepository;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(UpdateMoodEntryCommand request, CancellationToken ct)
        {
            var mood = await _moodRepository.GetByJournalEntryIdAsync(request.Id, ct);

            if (mood is null || mood.UserId != _currentUser.Id)
            {
                return false;
            }

            mood.UpdateUserScore(request.Score, request.Note);

            await _moodRepository.UpdateAsync(mood, ct);
            await _moodRepository.SaveChangesAsync(ct);

            return true;
        }
    }
}
