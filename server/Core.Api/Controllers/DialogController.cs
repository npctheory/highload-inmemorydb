using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MediatR;
using System.Security.Claims;
using Core.Application.Dialogs.Commands.SendMessage;
using Core.Application.Dialogs.Queries.ListMessages;
using Core.Application.Dialogs.DTO;
using Core.Application.Dialogs.Queries.ListDialogs;

namespace Core.Api.Controllers
{
    [ApiController]
    public class DialogController : ControllerBase
    {
        private readonly ISender _mediator;

        public DialogController(ISender mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("dialog/list")]
        public async Task<IActionResult> ListDialogs()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                var dialogs = await _mediator.Send(new ListDialogsQuery(userId));
                return Ok(dialogs);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet("dialog/{agentId}/list")]
        public async Task<IActionResult> ListMessages([FromRoute] string agentId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                var messages = await _mediator.Send(new ListMessagesQuery(userId, agentId));
                return Ok(messages);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpPost("dialog/{receiverId}/send")]
        public async Task<IActionResult> SendMessage([FromRoute] string receiverId, [FromBody] JsonElement jsonElement)
        {
            var senderId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string text = jsonElement.GetProperty("text").GetString();

            try
            {
                var message = await _mediator.Send(new SendMessageCommand(senderId, receiverId, text));
                return Ok(message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}