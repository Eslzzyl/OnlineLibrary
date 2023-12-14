using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace OnlineLibrary.Controller;

[Route("/admin")]
[ApiController]
public class AdminController(
    ILogger<AdminController> logger,
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApiUser> userManager)
    : ControllerBase
{
    private string GetAdminId()
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

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("users")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<UserResponseDto[]>> GetUsers(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "UserName",
        string? sortOrder = "ASC",
        string? filterQuery = null)
    {
        if (pageIndex < 0)
            pageIndex = 0;
        var query = context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(
                x => x.UserName.Contains(filterQuery) ||
                    x.Email.Contains(filterQuery));
        var recordCount = await query.CountAsync();
        query = (IQueryable<ApiUser>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var users = await query.ToListAsync();
        var userDtos = new List<UserResponseDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userDto = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList(),
                Avatar = user.Avatar
            };
            userDtos.Add(userDto);
        }
        logger.LogInformation("Admin {AdminId} is querying users.", GetAdminId());
        return new ResultDto<UserResponseDto[]>
        {
            Code = 0,
            Message = "OK",
            Data = userDtos.ToArray(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("user")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<ApiUser>> RemoveUser(string userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return new ResultDto<ApiUser>
            {
                Code = 1,
                Message = "User not found.",
                Data = null
            };
        // 如果用户是管理员，不能删除
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(RoleNames.Admin))
            return new ResultDto<ApiUser>
            {
                Code = 1,
                Message = "Cannot remove admin user.",
                Data = null
            };
        // 如果用户名是DeletedUser，不能删除
        if (user.UserName == "DeletedUser")
            return new ResultDto<ApiUser>
            {
                Code = 1,
                Message = "Cannot remove the user 'DeletedUser' because this is a special user.",
                Data = null
            };
        context.Users.Remove(user);

        // 将该用户的各种记录移动到 DeletedUser 名下
        var deletedUser = await userManager.FindByNameAsync("DeletedUser");
        if (deletedUser == null)
        {
            deletedUser = new ApiUser
            {
                UserName = "DeletedUser",
                Email = "deleted@oneline-library.org",
                EmailConfirmed = true,
                PhoneNumber = "00000000000",
                PhoneNumberConfirmed = true
            };
            await userManager.CreateAsync(deletedUser);
        }
        var histories = await context.BorrowHistories.Where(x => x.UserId == userId)
            .Include(borrowHistory => borrowHistory.Book).ToListAsync();
        context.RemoveRange(histories);
        foreach (var history in histories)
            context.BorrowHistories.Add(new BorrowHistory
            {
                UserId = deletedUser.Id,
                BookId = history.BookId,
                BorrowDate = history.BorrowDate,
                ReturnDate = history.ReturnDate,
                Book = history.Book,
                User = deletedUser
            });
        var current = await context.CurrentBorrows.Where(x => x.UserId == userId)
            .Include(currentBorrow => currentBorrow.Book).ToListAsync();
        context.RemoveRange(current);
        foreach (var borrow in current)
            context.CurrentBorrows.Add(new CurrentBorrow
            {
                UserId = deletedUser.Id,
                BookId = borrow.BookId,
                BorrowDate = borrow.BorrowDate,
                Book = borrow.Book,
                User = deletedUser
            });
        var recommends = await context.Recommends.Where(x => x.UserId == userId).ToListAsync();
        context.RemoveRange(recommends);
        foreach (var recommend in recommends)
            context.Recommends.Add(new Recommend
            {
                UserId = deletedUser.Id,
                AdminId = recommend.AdminId,
                Title = recommend.Title,
                Author = recommend.Author,
                Publisher = recommend.Publisher,
                Isbn = recommend.Isbn,
                UserRemark = recommend.UserRemark,
                AdminRemark = recommend.AdminRemark,
                CreateTime = recommend.CreateTime,
                UpdateTime = recommend.UpdateTime
            });
        var bookComments = await context.BookComments.Where(x => x.UserId == userId).ToListAsync();
        context.RemoveRange(bookComments);
        foreach (var comment in bookComments)
            context.BookComments.Add(new BookComment
            {
                UserId = deletedUser.Id,
                BookId = comment.BookId,
                RefCommentId = comment.RefCommentId,
                Content = comment.Content,
                CreateTime = comment.CreateTime
            });

        await context.SaveChangesAsync();

        logger.LogInformation("Admin {AdminId} removed user {UserId}.", GetAdminId(), userId);
        if (histories.Count > 0 || current.Count > 0 || recommends.Count > 0)
            logger.LogWarning(
                "Because user {UserId} is removed, the following records was removed too: {Histories}, {Current}, {Recommends}",
                userId, histories, current, recommends);

        return new ResultDto<ApiUser>
        {
            Code = 0,
            Message = "Successfully removed user.",
            Data = user
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("settings")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Setting>> GetSettings()
    {
        var settings = await context.Settings.FirstOrDefaultAsync();
        if (settings == null)
            return new ResultDto<Setting>
            {
                Code = 1,
                Message = "Settings not found.",
                Data = null
            };

        logger.LogInformation("Admin {AdminId} is querying settings.", GetAdminId());
        return new ResultDto<Setting>
        {
            Code = 0,
            Message = "OK",
            Data = settings
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("settings")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Setting>> UpdateSettings(uint borrowLimit, uint borrowDurationDays)
    {
        var settings = await context.Settings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new Setting
            {
                BorrowLimit = borrowLimit,
                BorrowDurationDays = borrowDurationDays
            };
            await context.Settings.AddAsync(settings);
        }
        else
        {
            settings.BorrowLimit = borrowLimit;
            settings.BorrowDurationDays = borrowDurationDays;
        }
        await context.SaveChangesAsync();

        logger.LogInformation("Admin {AdminId} updated settings.", GetAdminId());
        return new ResultDto<Setting>
        {
            Code = 0,
            Message = "OK",
            Data = settings
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("recommend")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Recommend>> UpdateRecommend(int recommendId, string adminRemark)
    {
        var recommend = await context.Recommends.FindAsync(recommendId);
        if (recommend == null)
            return new ResultDto<Recommend>
            {
                Code = 1,
                Message = "Recommend not found.",
                Data = null
            };

        recommend.IsProcessed = true;
        recommend.AdminId = GetAdminId();
        recommend.AdminRemark = adminRemark;
        recommend.UpdateTime = DateTime.Now;
        await context.SaveChangesAsync();

        logger.LogInformation("Admin {AdminId} updated recommend {RecommendId}.", GetAdminId(), recommendId);

        return new ResultDto<Recommend>
        {
            Code = 0,
            Message = "OK",
            Data = recommend
        };
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("statistics")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<AdminStatisticsDto>> GetStatistics()
    {
        var currentBorrows = await context.CurrentBorrows.ToListAsync();
        var borrowHistories = await context.BorrowHistories
            .Include(x => x.Book)
            .ToListAsync();

        var totalCurrentBorrowedBooks = currentBorrows.Count;
        var totalHistoryBorrowedBooks = borrowHistories.Count;

        var totalUsers = await context.Users.CountAsync();

        var past12Months = Enumerable.Range(0, 12)
            .Select(i => DateTime.Now.AddMonths(-i).ToString("yyyy-MM"))
            .Reverse();
        var monthlyBorrowedBooks = new Dictionary<string, int>();
        foreach (var month in past12Months)
        {
            var count = borrowHistories.Count(x => x.BorrowDate.ToString("yyyy-MM") == month);
            monthlyBorrowedBooks.Add(month, count);
        }

        var zhongTuClassification = new ZhongTuClassification();
        var borrowedBooksByClassification = borrowHistories
            .GroupBy(x => x.Book.Identifier[..1])
            .Select(x => new
            {
                Classification = zhongTuClassification.GetClassificationName(x.Key),
                Count = x.Count()
            })
            .OrderBy(x => x.Classification)
            .ToDictionary(g => g.Classification, g => g.Count);

        double averageBorrowDuration;
        if (totalHistoryBorrowedBooks == 0)
            averageBorrowDuration = 0;
        else
            averageBorrowDuration = borrowHistories.Average(x => (x.ReturnDate - x.BorrowDate).TotalDays);

        var unhandledRecommends = await context.Recommends.CountAsync(x => !x.IsProcessed);

        logger.LogInformation("Admin {AdminId} is querying statistics: {Info}", GetAdminId(), new
        {
            totalCurrentBorrowedBooks,
            totalHistoryBorrowedBooks,
            totalUsers,
            monthlyBorrowedBooks,
            borrowedBooksByClassification,
            averageBorrowDuration,
            unhandledRecommends
        });

        return new ResultDto<AdminStatisticsDto>
        {
            Code = 0,
            Message = "OK",
            Data = new AdminStatisticsDto
            {
                TotalHistoryBorrowedBooks = totalHistoryBorrowedBooks,
                TotalCurrentBorrowedBooks = totalCurrentBorrowedBooks,
                TotalUsers = totalUsers,
                MonthlyBorrowedBooks = monthlyBorrowedBooks,
                BorrowedBooksByClassification = borrowedBooksByClassification,
                AverageBorrowDuration = averageBorrowDuration.ToString("F2"),
                UnhandledRecommends = unhandledRecommends
            }
        };
    }
}