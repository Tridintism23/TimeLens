using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeLens.Application.Journals.Commands;
using TimeLens.Application.Journals.Queries;

namespace TimeLens.WebAPI.Controllers;

[ApiController]
[Route("api/journals")]
[Authorize]
public class JournalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JournalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/journals
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entries = await _mediator.Send(new GetJournalEntriesQuery());
        return Ok(entries);
    }

    // GET /api/journals/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entry = await _mediator.Send(new GetJournalEntryByIdQuery(id));

        if (entry is null)
            return NotFound();

        return Ok(entry);
    }

    // POST /api/journals
    [HttpPost]
    public async Task<IActionResult> Create(CreateJournalEntryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    // PUT /api/journals/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateJournalEntryCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id không khớp.");

        var success = await _mediator.Send(command);

        if (!success)
            return NotFound();

        return NoContent();
    }
}