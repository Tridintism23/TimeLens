using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Moods.Queries
{
    public record GetMoodTrendQuery(DateTime From, DateTime To) : IRequest<List<MoodEntry>>;

    public class GetMoodTrendQueryHandler : IRequestHandler<GetMoodTrendQuery, List<MoodEntry>>
    {
        private readonly IMoodRepository _moodRepository;
        private readonly ICurrentUser _currentUser;

        public GetMoodTrendQueryHandler(IMoodRepository moodRepository, ICurrentUser currentUser)
        {
            _moodRepository = moodRepository;
            _currentUser = currentUser;
        }

        public async Task<List<MoodEntry>> Handle(GetMoodTrendQuery request, CancellationToken ct)
        {
            return await _moodRepository.GetByDateRangeAsync(
                _currentUser.Id,
                request.From,
                request.To,
                ct);
        }
    }
}
