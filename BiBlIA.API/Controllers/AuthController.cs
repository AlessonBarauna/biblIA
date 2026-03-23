using BíblIA.Api.Models;
using BíblIA.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BíblIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        
        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }
}