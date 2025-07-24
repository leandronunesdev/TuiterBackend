using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var mailAddress = new MailAddress(dto.Email);
        }
        catch
        {
            return BadRequest("Invalid email format.");
        }

        var existingUser = await _userService.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest("Email already in use.");

        if (string.IsNullOrWhiteSpace(dto.Username) ||
            dto.Username.Length < 4 ||
            dto.Username.Length > 15 ||
            !System.Text.RegularExpressions.Regex.IsMatch(dto.Username, @"^[A-Za-z0-9_]+$"))
        {
            return BadRequest("Invalid username. It must be 4-15 characters long and contain only letters, numbers, and underscores.");
        }

        var existingUserByUsername = await _userService.GetByUsernameAsync(dto.Username);
        if (existingUserByUsername != null)
            return BadRequest("Username already in use.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            return BadRequest("Invalid password. It must be at least 8 characters long.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
        };

        await _userService.CreateAsync(newUser);
        return Ok(new { newUser.Id, newUser.Username, newUser.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto, [FromServices] JwtHelper jwtHelper)
    {
        var user = await _userService.GetByUsernameOrEmailAsync(dto.UsernameOrEmail);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = jwtHelper.GenerateToken(user);

        return Ok(new
        {
            Token = token,
            User = new { user.Id, user.Username, user.Email }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email
        });
    }
}