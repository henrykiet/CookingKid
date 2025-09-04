using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Cooking_kid_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var result = _authService.GetAllAsync();
            return Ok(result);
        }


    }
}
