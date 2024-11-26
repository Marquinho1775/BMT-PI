using BMT_backend.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchProductsAndEnterprisesQuery _searchProductsAndEnterprisesQuerie;

        public SearchController(SearchProductsAndEnterprisesQuery searchProductsAndEnterprisesQuerie)
        {
            _searchProductsAndEnterprisesQuerie = searchProductsAndEnterprisesQuerie;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProductsAndEnterprisesAsync(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
                return BadRequest(new { Message = "La entrada del usuario no puede ser nula." });
            try
            {
                var resultDto = await _searchProductsAndEnterprisesQuerie.SearchProductsAndEnterprisesAsync(userInput);
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
