using Application.Boudary.User;
using Application.Command.Authentication;
using Application.Command.User;
using Infrastructure.Message.Interface;
using Microsoft.AspNetCore.Mvc;

namespace TestePraticoCsharpMarkus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMessagesHandler _messagesHandler;

        public UserController(ILogger<UserController> logger, IMessagesHandler messagesHandler)
        {
            _logger = logger;
            _messagesHandler = messagesHandler;
        }

        [HttpPost("")]
        public async Task<IActionResult> Register(CreateUserInput input)
        {
            var response = await _messagesHandler.SendCommandAsync<CreateUserCommand, CreateUserOutput>
                     (
                        new CreateUserCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationInput input)
        {
            var response = await _messagesHandler.SendCommandAsync<AuthenticationCommand, AuthenticationOutput>
                     (
                        new AuthenticationCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
