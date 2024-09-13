using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MediatR;
using System.Security.Claims;
using Core.Application.Friends.Queries.ListFriends;
using Core.Application.Friends.Commands.AddFriend;
using Core.Application.Friends.Commands.DeleteFriend;

namespace Core.Api.Controllers;

[ApiController]
public class FriendController : ControllerBase
{
    private readonly ISender _mediator;

    public FriendController(ISender mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("friend/list")]
    public async Task<IActionResult> GetFriends()
    {
        var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        try
        {
            var friends = await _mediator.Send(new ListFriendsQuery(user_id));
            return Ok(friends);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPut("friend/delete/{friend_id}")]
    public async Task<IActionResult> DeleteFriend([FromRoute] string friend_id)
    {
        var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        try
        {
            await _mediator.Send(new DeleteFriendQuery(user_id, friend_id));
            return Ok(await _mediator.Send(new ListFriendsQuery(user_id)));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPut("friend/set/{friend_id}")]
    public async Task<IActionResult> SetFriend([FromRoute] string friend_id)
    {
        var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        try
        {
            await _mediator.Send(new AddFriendQuery(user_id, friend_id));
            return Ok(await _mediator.Send(new ListFriendsQuery(user_id)));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}