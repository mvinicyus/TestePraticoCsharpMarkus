using Application.Boudary.Post;
using Application.Command.Post;
using Infrastructure.Message.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestePraticoCsharpMarkus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IMessagesHandler _messagesHandler;

        public BlogController(ILogger<BlogController> logger,
                              IMessagesHandler messagesHandler)
        {
            _logger = logger;
            _messagesHandler = messagesHandler;
        }

        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> Post(CreatePostInput input)
        {
            var response = await _messagesHandler.SendCommandAsync<CreatePostCommand, CreatePostOutput>
                     (
                        new CreatePostCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("")]
        public async Task<IActionResult> Put(UpdatePostInput input)
        {
            var response = await _messagesHandler.SendCommandAsync<UpdatePostCommand, UpdatePostOutput>
                     (
                        new UpdatePostCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var input = new DeletePostInput
            {
                Id = id
            };
            var response = await _messagesHandler.SendCommandAsync<DeletePostCommand, DeletePostOutput>
                     (
                        new DeletePostCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetPostsInput input)
        {
            var response = await _messagesHandler.SendCommandAsync<GetPostsCommand, GetPostsOutput>
                     (
                        new GetPostsCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var input = new GetPostInput
            {
                Id = id
            };
            var response = await _messagesHandler.SendCommandAsync<GetPostCommand, GetPostOutput>
                     (
                        new GetPostCommand { Input = input }
                     ).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
