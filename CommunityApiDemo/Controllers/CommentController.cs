using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunityApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICommentRepo _repo;
        public CommentController(ICommentRepo repo, ILogger<CommentController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("GetComments")]
        public async Task<IActionResult> GetComments(int postID)
        {
            try
            {
                List<Comment> result = await _repo.GetComments(postID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Comment-Search method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create(int postID, string content)
        {
            try
            {
                bool result = await _repo.Create(postID, content);
                if (!result)
                {
                    return Unauthorized("You cannot comment on a post you have created.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Comment-Create method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
