using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Users.Queries.GetUser;
using Core.Application.Users.Queries.SearchUsers;
using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Users.DTO;
using System.Text.Json;
using Core.Application.Users.Queries.Login;
using Core.Application.Users.Commands.Register;

namespace Core.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public UserController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("user/get/{id}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            UserDTO user = await _mediator.Send(new GetUserQuery(id));
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("user/search")]
        public async Task<IActionResult> SearchUsersAsync([FromQuery] string first_name, [FromQuery] string second_name)
        {
            List<UserDTO> users = await _mediator.Send(new SearchUsersQuery(first_name, second_name));
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement jsonElement)
        {
            string id = jsonElement.GetProperty("id").GetString();
            string password = jsonElement.GetProperty("password").GetString();

            TokenDTO token = await _mediator.Send(new LoginQuery(id,password)); 
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JsonElement jsonElement)
        {
            string first_name = jsonElement.GetProperty("first_name").GetString();
            string second_name = jsonElement.GetProperty("second_name").GetString();
            string birthdate = jsonElement.GetProperty("birthdate").GetString();
            string biography = jsonElement.GetProperty("biography").GetString();
            string city = jsonElement.GetProperty("city").GetString();
            string password = jsonElement.GetProperty("password").GetString();

            UserDTO user = await _mediator.Send(new RegisterCommand(first_name,second_name,birthdate,biography,city,password)); 
            return Ok(user);
        }
    }
}