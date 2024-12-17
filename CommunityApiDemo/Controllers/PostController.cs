using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunityApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IPostRepo _repo;
        public PostController(IPostRepo repo, ILogger<PostController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? title = null, int? category = null)
        {
            try
            {
                List<Post> result = await _repo.Search(title, category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Post-Search method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create(string title, string content, int categoryid)
        {
            try
            {
                bool result = await _repo.Create(title, content, categoryid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Post-Create method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update(int postID, string content)
        {
            try
            {
                bool result = await _repo.Update(postID, content);
                if (!result)
                {
                    return Unauthorized("You cannot update a post you haven't created");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Post-Update method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int postID)
        {
            try
            {
                bool result = await _repo.Delete(postID);
                if (!result)
                {
                    return Unauthorized("You cannot delete a post you haven't created");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Post-Delete method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
