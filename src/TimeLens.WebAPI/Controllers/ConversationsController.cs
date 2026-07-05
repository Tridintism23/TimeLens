using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeLens.Application.Conversations.Commands;
using TimeLens.Application.Conversations.Queries;

namespace TimeLens.WebAPI.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationsController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        // GET /api/conversations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conversations = await _mediator.Send(new GetConversationQuery());

            return Ok(conversations);
        }

        // GET /api/conversations/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var conversation = await _mediator.Send(new GetConversationByIdQuery(id));

            if (conversation is null)
            {
                return NotFound();
            }
            
            return Ok(conversation);
        }

        // POST /api/conversations
        [HttpPost]
        public async Task<IActionResult> Start(StartConversationCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // POST /api/conversations/{id}/mmessages
        [HttpPost("{id:guid}/messages")]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] string content)
        {
            var result = await _mediator.Send(new SendMessageCommand(id, content));

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { response = result.AiResponse });
        }
    }
}
