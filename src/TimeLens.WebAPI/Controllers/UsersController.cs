using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeLens.Application.Users.Commands;

namespace TimeLens.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // PUT /api/users/philosophy
        [HttpPut("philosophy")]
        public async Task<IActionResult> UpdatePhilosophy(UpdatePhilosophyCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
