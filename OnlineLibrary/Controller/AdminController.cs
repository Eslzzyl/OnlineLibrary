using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using System.Linq.Dynamic.Core;

namespace OnlineLibrary.Controller;

[Route("/admin")]
[ApiController]
public class AdminController(ILogger<AdminController> logger, ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor, UserManager<ApiUser> userManager)
    : ControllerBase
{
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("users")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<UserResponseDto[]>> GetUsers(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "UserName",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        if (pageIndex < 0) {
            pageIndex = 0;
        }
        var query = context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.UserName.Contains(filterQuery) || 
                x.Email.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<ApiUser>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var users = await query.ToListAsync();
        var userDtos = new List<UserResponseDto>();
        foreach (var user in users) {
            var roles = await userManager.GetRolesAsync(user);
            var userDto = new UserResponseDto {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList(),
                Avatar = user.Avatar,
            };
            userDtos.Add(userDto);
        }
        return new ResultDto<UserResponseDto[]>() {
            Code = 0,
            Message = "OK",
            Data = userDtos.ToArray(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }
    
    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("user")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<ApiUser>> RemoveUser(string userId) {
        var user = await context.Users.FindAsync(userId);
        if (user == null) {
            return new ResultDto<ApiUser>() {
                Code = 1,
                Message = "User not found.",
                Data = null,
            };
        }
        // 如果用户是管理员，不能删除
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(RoleNames.Admin)) {
            return new ResultDto<ApiUser>() {
                Code = 1,
                Message = "Cannot remove admin user.",
                Data = null,
            };
        }
        context.Users.Remove(user);

        var histories = await context.BorrowHistories.Where(x => x.UserId == userId).ToListAsync();
        context.BorrowHistories.RemoveRange(histories);
        var current = await context.CurrentBorrows.Where(x => x.UserId == userId).ToListAsync();
        context.CurrentBorrows.RemoveRange(current);
        
        await context.SaveChangesAsync();
        
        return new ResultDto<ApiUser>() {
            Code = 0,
            Message = "Successfully removed user.",
            Data = user,
        };
    }
    
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("settings")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Setting>> GetSettings() {
        var settings = await context.Settings.FirstOrDefaultAsync();
        return new ResultDto<Setting>() {
            Code = 0,
            Message = "OK",
            Data = settings,
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("settings")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Setting>> UpdateSettings(uint borrowLimit, uint borrowDurationDays) {
        var settings = await context.Settings.FirstOrDefaultAsync();
        if (settings == null) {
            settings = new Setting() {
                BorrowLimit = borrowLimit,
                BorrowDurationDays = borrowDurationDays,
            };
            await context.Settings.AddAsync(settings);
        } else {
            settings.BorrowLimit = borrowLimit;
            settings.BorrowDurationDays = borrowDurationDays;
        }
        await context.SaveChangesAsync();
        return new ResultDto<Setting>() {
            Code = 0,
            Message = "OK",
            Data = settings,
        };
    }
}