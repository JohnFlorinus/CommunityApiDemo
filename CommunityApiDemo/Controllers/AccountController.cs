using CommunityApiDemo.Context;
using CommunityApiDemo.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CommunityApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAccountRepo _repo;

        public AccountController(IAccountRepo repo, ILogger<AccountController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string name, string password)
        {
            try
            {
                string result = await _repo.Login(name, password);
                if (result == "unauthorized")
                {
                    return Unauthorized("Invalid credentials");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Account-Login method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(string email, string name, string password)
        {
            try
            {
                bool result = await _repo.Register(email, name, password);
                // true = skapad, false = finns redan namn/email
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Account-Register method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string newpassword)
        {
            try
            {
                string result = await _repo.ChangePassword(newpassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Account-Update method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string password)
        {
            try
            {
                bool result = await _repo.Delete(password);
                if (!result)
                {
                    return Unauthorized("Wrong Password");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"API Error in Account-Delete method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
