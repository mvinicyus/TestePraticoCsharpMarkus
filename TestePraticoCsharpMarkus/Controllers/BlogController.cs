using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestePraticoCsharpMarkus.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(Enumerable.Range(1, 5).Select(index => new
            {
                Title = $"Post {index}",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Summary = "The start information for life",
                Body = "The start information for life is with the life is only one and, we need love we family"
            })
            .ToArray());
        }

        [HttpGet("/{id}")]
        public IActionResult Get(int id)
        {
            return Ok(new
            {
                Title = $"Post {id}",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
                Summary = "The start information for life",
                Body = "The start information for life is with the life is only one and, we need love we family"
            });
        }

        [HttpPost("")]
        public IActionResult Post(object post)
        {
            return Ok();
        }

        [HttpPut("")]
        public IActionResult Put(object post)
        {
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
