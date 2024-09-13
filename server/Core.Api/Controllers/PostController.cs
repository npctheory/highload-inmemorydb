using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MediatR;
using System.Security.Claims;
using Core.Application.Posts.Queries.ListPosts;
using Core.Application.Posts.Queries.GetPostFeed;
using Core.Application.Posts.Queries.GetPost;
using Core.Application.Posts.Commands.DeletePost;
using Core.Application.Posts.DTO;
using Core.Application.Posts.Commands.UpdatePost;
using Core.Application.Posts.Commands.CreatePost;

namespace Core.Api.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ISender _mediator;

        public PostController(ISender mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("post/list")]
        public async Task<IActionResult> ListPosts()
        {
            var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                var posts = await _mediator.Send(new ListPostsQuery(user_id));
                return Ok(posts);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet("post/feed")]
        public async Task<IActionResult> ListFriendPosts([FromQuery] int offset, [FromQuery] int limit)
        {
            var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                var posts = await _mediator.Send(new GetPostFeedQuery(user_id, offset, limit));
                return Ok(posts);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet("post/get/{id}")]
        public async Task<IActionResult> GetPostById([FromRoute] string id)
        {
           
            try
            {
                var posts = await _mediator.Send(new GetPostQuery(id));
                return Ok(posts);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut("post/delete/{post_id}")]
        public async Task<IActionResult> DeletePost([FromRoute] string post_id)
        {
            var user_id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            try
            {
                var result = await _mediator.Send(new DeletePostCommand(user_id, post_id));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

         [Authorize]
        [HttpPost("post/create")]
        public async Task<IActionResult> CreatePost([FromBody] JsonElement jsonElement)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string text = jsonElement.GetProperty("text").GetString();
            try
            {
                PostDTO post = await _mediator.Send(new CreatePostCommand(userId, text));
                return Ok(post);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut("post/update")]
        public async Task<IActionResult> UpdatePost([FromBody] JsonElement jsonElement)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string text = jsonElement.GetProperty("text").GetString();
            string id = jsonElement.GetProperty("id").GetString();

            try
            {
                var updated = await _mediator.Send(new UpdatePostCommand(userId, id, text));
                if (updated != null)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}