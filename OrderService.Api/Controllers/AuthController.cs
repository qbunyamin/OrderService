using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Services;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;

    public AuthController(JwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] string userId)
    {
        // Not: Bu sadece örnek, şifre kontrolü yok
        var token = _tokenService.GenerateToken(userId);
        return Ok(new { token });
    }
}
