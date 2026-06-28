using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeLens.Application.Moods.Commands;
using TimeLens.Application.Moods.Queries;

namespace TimeLens.WebAPI.Controllers
{
    [ApiController]
    [Route("api/moods")]
    [Authorize]
    public class MoodsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoodsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/moods
        [HttpPost]
        public async Task<IActionResult> Create(CreateMoodEntryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        //PUT /api/moods/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateMoodEntryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Id khong khop");
            }

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET /api/moods/trend?from=2026-01-01&to=2026-06-30
        [HttpGet("trend")]
        public async Task<IActionResult> GetTrend([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var moods = await _mediator.Send(new GetMoodTrendQuery(from, to));
            return Ok(moods);
        }
    }
}
