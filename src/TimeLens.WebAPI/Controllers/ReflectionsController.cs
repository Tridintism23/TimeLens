using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeLens.Application.Reflections.Commands;

namespace TimeLens.WebAPI.Controllers
{

    [ApiController]
    [Route("api/reflections")]
    [Authorize]
    public class ReflectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReflectionsController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        // POST /api/reflections/generate
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(GenerateReflectionCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage });

                return Ok(new { questions = result.Questions });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(429, new { message = ex.Message });
            }
        }
    }
}
