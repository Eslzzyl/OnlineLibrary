using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineLibrary.Controller;

[Route("[controller]/[action]")]
[ApiController]
public class AccountController(
    ILogger<AccountController> logger,
    IConfiguration configuration,
    UserManager<ApiUser> userManager,
    IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private string GetUserId()
    {
        // Get user id from token
        string userId;
        if (httpContextAccessor.HttpContext != null)
        {
            var claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
                userId = claim.Value;
            else
                throw new Exception();
        }
        else
        {
            throw new Exception();
        }
        return userId;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult> Register(RegisterRequestDto input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(input.UserName);
                if (user != null)
                    return StatusCode(StatusCodes.Status400BadRequest, "User already exists.");
                var newUser = new ApiUser
                {
                    UserName = input.UserName
                };
                var result = await userManager.CreateAsync(newUser, input.Password);
                if (result.Succeeded)
                {
                    // 新用户添加到普通用户组
                    await userManager.AddToRoleAsync(newUser, RoleNames.User);
                    logger.LogInformation("User {userName} ({email}) has been created.", newUser.UserName,
                        newUser.Email);
                    return StatusCode(StatusCodes.Status201Created, $"User '{newUser.UserName}' has been created.");
                }
                var description = result.Errors.Select(error => error.Description).FirstOrDefault();
                logger.LogWarning("User creation failed: {description}", string.Join(" ", description));
                throw new Exception($"Error: {string.Join(" ", description)}");
            }
            logger.LogWarning("User creation failed: {description}", string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
            var details = new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(details);
        }
        catch (Exception e)
        {
            logger.LogWarning("User creation failed: {description}", e.Message);
            var exceptionDetails = new ProblemDetails
            {
                Detail = e.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, exceptionDetails);
        }
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult> Login(LoginRequestDto input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // 支持用户使用用户名和邮箱登录
                var user = await userManager.FindByNameAsync(input.Account) ??
                    await userManager.FindByEmailAsync(input.Account);
                if (user == null)
                    return StatusCode(StatusCodes.Status404NotFound, "User not found.");
                if (user.UserName == "DeletedUser")
                    return StatusCode(StatusCodes.Status400BadRequest, "You are trying to login as a special user.");
                if (!await userManager.CheckPasswordAsync(user, input.Password))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Incorrect password.");
                }
                var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["JWT:SigningKey"])),
                    SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                claims.Add(new Claim(
                    ClaimTypes.NameIdentifier, user.Id));
                claims.AddRange(
                    (await userManager.GetRolesAsync(user))
                    .Select(r => new Claim(ClaimTypes.Role, r)));

                var jwtObject = new JwtSecurityToken(
                    configuration["JWT:Issuer"],
                    configuration["JWT:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(300),
                    signingCredentials: signingCredentials);

                var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);
                var avatar = user.Avatar;
                var roles = await userManager.GetRolesAsync(user);
                string role;
                if (roles.Contains("Admin"))
                    role = "Admin";
                else if (roles.Contains("Moderator"))
                    role = "Moderator";
                else
                    role = "User";

                logger.LogInformation("User {userName} ({email}) has logged in.", user.UserName, user.Email);

                return StatusCode(StatusCodes.Status200OK, new LoginResponseDto
                {
                    Token = jwtString,
                    UserName = user.UserName,
                    Role = role,
                    Avatar = avatar
                });
            }
            logger.LogWarning("User login failed: {description}", string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
            var details = new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(details);
        }
        catch (Exception e)
        {
            logger.LogWarning("User login failed: {description}", e.Message);
            var exceptionDetails = new ProblemDetails
            {
                Detail = e.Message,
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return StatusCode(StatusCodes.Status401Unauthorized, exceptionDetails);
        }
    }

    [HttpPut]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult> UpdateInfo(UpdateInfoRequestDto input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(GetUserId());
                if (user == null)
                    return StatusCode(StatusCodes.Status404NotFound, "User not found.");
                user.Email = input.Email;
                user.PhoneNumber = input.PhoneNumber;
                user.Avatar = input.Avatar;
                if (input.Password != "")
                {
                    await userManager.RemovePasswordAsync(user);
                    await userManager.AddPasswordAsync(user, input.Password);
                }
                await userManager.UpdateAsync(user);
                logger.LogInformation("User {userName} ({email}) has updated info.", user.UserName, user.Email);
                return StatusCode(StatusCodes.Status200OK, "User info updated.");
            }
            logger.LogWarning("User info update failed: {description}", string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
            var details = new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(details);
        }
        catch (Exception e)
        {
            logger.LogWarning("User info update failed: {description}", e.Message);
            var exceptionDetails = new ProblemDetails
            {
                Detail = e.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, exceptionDetails);
        }
    }

    [HttpGet]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<GetInfoResponseDto>> GetInfo()
    {
        var user = await userManager.FindByIdAsync(GetUserId());
        if (user == null)
            return new ResultDto<GetInfoResponseDto>
            {
                Code = 1,
                Message = "Couldn't find user",
                Data = null
            };
        logger.LogInformation("User {userName} ({email}) has got info.", user.UserName, user.Email);
        return new ResultDto<GetInfoResponseDto>
        {
            Code = 0,
            Message = "OK",
            Data = new GetInfoResponseDto
            {
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Avatar = user.Avatar ?? string.Empty
            }
        };
    }
}