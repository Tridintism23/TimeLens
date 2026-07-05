using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;

namespace TimeLens.Application.Reflections.Queries
{
    public record GetWeeklyInsightQuery(DateTime From, DateTime To) : IRequest<string>;

    public class GetWeeklyInsightQueryHandler : IRequestHandler<GetWeeklyInsightQuery, string>
    {
        private readonly ReflectionEngine _reflectionEngine;
        private readonly ICurrentUser _currentUser;

        public GetWeeklyInsightQueryHandler(ReflectionEngine reflectionEngine, ICurrentUser currentUser)
        {
            _reflectionEngine = reflectionEngine;
            _currentUser = currentUser;
        }

        public async Task<string> Handle(GetWeeklyInsightQuery request, CancellationToken ct)
        {
            return await _reflectionEngine.GenerateWeeklyInsightAsync(
                _currentUser.Id, request.From, request.To, ct);
        }
    }
}
