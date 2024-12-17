using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunityApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICategoryRepo _repo;
        public CategoryController(ICategoryRepo repo, ILogger<CategoryController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("GetCategoryIDs")]
        public async Task<IActionResult> GetCategoryIDs()
        {
            try
            {
                List<Category> result = await _repo.GetIDs();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Category-GetCategoryIDs method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
