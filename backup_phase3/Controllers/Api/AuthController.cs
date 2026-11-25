using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Api;
using ProductCare.Web.ViewModels.Common;

namespace ProductCare.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates user credentials and issues a signed JWT Bearer token for API testing.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid login request", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid credentials or user account deactivated."));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid email or password."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

        var response = new LoginResponseDto
        {
            Token = token,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles.ToList(),
            ExpiresAtUtc = DateTime.UtcNow.AddHours(2)
        };

        _logger.LogInformation("API login successful for user {Email}", user.Email);

        return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Authentication successful."));
    }
}
