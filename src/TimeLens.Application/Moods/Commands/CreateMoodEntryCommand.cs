using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Moods.Commands
{
    public record CreateMoodEntryCommand(int UserScore, string? Note, Guid? JournalEntryId) : IRequest<Guid>;

    public class CreateMoodEntryCommandHandler : IRequestHandler<CreateMoodEntryCommand, Guid>
    {
        private readonly IMoodRepository _moodRepository;
        private readonly ICurrentUser _currentUser;

        public CreateMoodEntryCommandHandler(IMoodRepository moodRepository, ICurrentUser currentUser)
        {
            _moodRepository = moodRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateMoodEntryCommand request, CancellationToken ct)
        {
            var mood = MoodEntry.CreateFromUser(
                _currentUser.Id, 
                request.UserScore, 
                request.Note, 
                request.JournalEntryId);

            await _moodRepository.AddAsync(mood, ct);
            await _moodRepository.SaveChangesAsync(ct);

            return mood.Id;
        }
    }
}
