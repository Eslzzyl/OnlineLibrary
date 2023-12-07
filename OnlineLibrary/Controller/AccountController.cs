using OnlineLibrary.Constant;

namespace OnlineLibrary.Controller;

using Microsoft.AspNetCore.Mvc;
using Dto;
using Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("[controller]/[action]")]
[ApiController]
public class AccountController(ApplicationDbContext context,
        ILogger<AccountController> logger,
        IConfiguration configuration,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager)
    : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly SignInManager<ApiUser> _signInManager = signInManager;

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult> Register(RegisterRequestDto input) {
        try {
            if (ModelState.IsValid) {
                var newUser = new ApiUser {
                    UserName = input.UserName,
                };
                var result = await userManager.CreateAsync(newUser, input.Password);
                if (result.Succeeded) {
                    // 新用户添加到普通用户组
                    await userManager.AddToRoleAsync(newUser, RoleNames.User);
                    logger.LogInformation("User {userName} ({email}) has been created.", newUser.UserName,
                        newUser.Email);
                    return StatusCode(201, $"User '{newUser.UserName}' has been created.");
                }
                else {
                    throw new Exception(
                        $"Error: {string.Join(" ",
                            result.Errors.Select(error => error.Description))}");
                }
            }
            else {
                var details = new ValidationProblemDetails(ModelState) {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest
                };
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e) {
            var exceptionDetails = new ProblemDetails {
                Detail = e.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, exceptionDetails);
        }
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult> Login(LoginRequestDto input) {
        try {
            if (ModelState.IsValid) {
                // 支持用户使用用户名和邮箱登录
                var user = await userManager.FindByNameAsync(input.Account) ?? await userManager.FindByEmailAsync(input.Account);
                if (user == null) {
                    throw new Exception("User account not found.");
                }
                else if (!await userManager.CheckPasswordAsync(user, input.Password)) {
                    throw new Exception("Invalid password.");
                }
                else {
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                            configuration["JWT:SigningKey"])),
                        SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>();
                    claims.Add(new Claim(
                        ClaimTypes.NameIdentifier, user.Id));
                    claims.AddRange(
                        (await userManager.GetRolesAsync(user))
                        .Select(r => new Claim(ClaimTypes.Role, r)));

                    var jwtObject = new JwtSecurityToken(
                        issuer: configuration["JWT:Issuer"],
                        audience: configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(300),
                        signingCredentials: signingCredentials);

                    var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);
                    var roles = await userManager.GetRolesAsync(user);
                    string role;
                    if (roles.Contains("Admin")) {
                        role = "Admin";
                    }
                    else if (roles.Contains("Moderator")) {
                        role = "Moderator";
                    }
                    else {
                        role = "User";
                    }

                    return StatusCode(StatusCodes.Status200OK, new LoginResponseDto() {
                        Token = jwtString,
                        UserName = user.UserName,
                        Role = role
                    });
                }
            }
            else {
                var details = new ValidationProblemDetails(ModelState) {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest
                };
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e) {
            var exceptionDetails = new ProblemDetails {
                Detail = e.Message,
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return StatusCode(StatusCodes.Status401Unauthorized, exceptionDetails);
        }
    }
}