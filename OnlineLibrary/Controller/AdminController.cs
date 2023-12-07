namespace OnlineLibrary.Controller;

using System.Linq.Dynamic.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Model;
using Dto;
using Constant;

[Route("/admin")]
[ApiController]
public class AdminController(ILogger<BookController> logger, ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private string GetUserId() {
        // Get user id from token
        string userId;
        if (httpContextAccessor.HttpContext != null) {
            var claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null) {
                userId = claim.Value;
            } else {
                throw new Exception();
            }
        } else {
            throw new Exception();
        }
        return userId;
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("users")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<ApiUser[]>> GetUsers(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "UserName",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        var query = context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.UserName.Contains(filterQuery) || 
                x.Email.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<ApiUser>)query
            .OrderBy($"{sortColumn} {sortOrder}")
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
        return new ResultDto<ApiUser[]>() {
            Code = 0,
            Message = "OK",
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }
    
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("user")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<ApiUser>> RemoveUser(string userId) {
        var user = await context.Users.FindAsync(userId);
        if (user == null) {
            throw new Exception("User not found.");
        }
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return new ResultDto<ApiUser>() {
            Code = 0,
            Message = $"Successfully removed user.",
            Data = user,
        };
    }
}