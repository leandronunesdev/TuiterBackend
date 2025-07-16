using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Register")]
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

        var existingUserByUsername = await _userService.GetByUsernameAsync(dto.Username);
        if (existingUserByUsername != null)
            return BadRequest("Username already in use.");

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

    [HttpPost("Login")]
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
}