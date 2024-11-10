using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagHandler _tagHandler;

        public TagController(IConfiguration configuration)
        {
            _tagHandler = new TagHandler(configuration);
        }

        [HttpGet]
        public List<Tag> GetTags()
        {
            try
            {
                var tags = _tagHandler.GetTags();
                return tags;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateTag(string tag)
        {
            try
            {
                if (tag == null)
                {
                    return BadRequest("Tag information is not valid.");
                }
                var result = _tagHandler.CreateTag(tag);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the tag");
            }
        }
    }
}
