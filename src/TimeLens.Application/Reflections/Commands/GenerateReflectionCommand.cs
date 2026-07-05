using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;

namespace TimeLens.Application.Reflections.Commands;

public record GenerateReflectionCommand(
    Guid JournalEntryId) : IRequest<GenerateReflectionResult>;

public record GenerateReflectionResult(
    bool Success,
    string? Questions,
    string? ErrorMessage);

public class GenerateReflectionCommandHandler
    : IRequestHandler<GenerateReflectionCommand, GenerateReflectionResult>
{
    private readonly ReflectionEngine _reflectionEngine;
    private readonly ICurrentUser _currentUser;

    public GenerateReflectionCommandHandler(
        ReflectionEngine reflectionEngine,
        ICurrentUser currentUser)
    {
        _reflectionEngine = reflectionEngine;
        _currentUser = currentUser;
    }

    public async Task<GenerateReflectionResult> Handle(
        GenerateReflectionCommand request, CancellationToken ct)
    {
        // Handler chỉ cần 1 dòng — Facade lo hết phần còn lại
        var result = await _reflectionEngine.GenerateAsync(
            request.JournalEntryId,
            _currentUser.Id,
            ct);

        return new GenerateReflectionResult(
            result.Success,
            result.Questions,
            result.ErrorMessage);
    }
}
