using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace OnlineLibrary.Controller;

[Route("/user")]
[ApiController]
public class UserController(
    ILogger<UserController> logger,
    ApplicationDbContext context,
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

    [Authorize]
    [HttpGet("currentborrow")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<BorrowDto[]>> GetCurrentBorrow(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null)
    {
        var userId = GetUserId();
        var query = context.CurrentBorrows
            .Include(x => x.Book)
            .AsQueryable();
        query = query.Where(x => x.UserId == userId);
        if (!string.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) ||
                    x.Book.Author.Contains(filterQuery) ||
                    x.Book.Publisher.Contains(filterQuery));
        var recordCount = await query.CountAsync();
        query = (IQueryable<CurrentBorrow>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto
        {
            Id = x.Book.Id,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            BorrowDuration = (DateTime.Now - x.BorrowDate).TotalDays.ToString("F2", CultureInfo.InvariantCulture),
            ReturnDate = null
        }).ToArrayAsync();
        return new ResultDto<BorrowDto[]>
        {
            Code = 0,
            Message = "OK",
            Data = dto,
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount
        };
    }

    [Authorize]
    [HttpGet("borrowhistory")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<BorrowDto[]>> GetBorrowHistory(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null)
    {
        var userId = GetUserId();
        var query = context.BorrowHistories
            .Include(x => x.Book)
            .AsQueryable();
        query = query.Where(x => x.UserId == userId);
        if (!string.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) ||
                    x.Book.Author.Contains(filterQuery) ||
                    x.Book.Publisher.Contains(filterQuery));
        var recordCount = await query.CountAsync();
        query = (IQueryable<BorrowHistory>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto
        {
            Id = x.Book.Id,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            ReturnDate = x.ReturnDate.Date.ToString("yyyy-MM-dd"),
            BorrowDuration = (x.ReturnDate - x.BorrowDate).TotalDays.ToString("F2")
        }).ToArrayAsync();
        return new ResultDto<BorrowDto[]>
        {
            Code = 0,
            Message = "OK",
            Data = dto,
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount
        };
    }

    [HttpPost("borrow")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> BorrowBook(int bookId)
    {
        var userId = GetUserId();

        var user = await context.Users.FindAsync(userId);
        var book = await context.Books.FindAsync(bookId);

        if (user == null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "User Not Found",
                Data = null
            };

        if (book == null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "Book Not Found",
                Data = null
            };

        // 检查是否达到最大借书量
        var setting = await context.Settings.FirstOrDefaultAsync();
        var cb = context.CurrentBorrows.Where(x => x.UserId == userId);
        if (setting.BorrowLimit != 0 && cb.Count() >= setting.BorrowLimit)
            return new ResultDto<Book>
            {
                Code = 2,
                Message = $"You have achieved the borrow limit of {setting.BorrowLimit}",
                Data = null
            };

        // 检查用户当前借书表中是否有超期
        var cbList = await cb.ToListAsync();
        var ifExceed = cbList.Any(x => (DateTime.Now - x.BorrowDate).TotalDays > setting.BorrowDurationDays);
        if (setting.BorrowDurationDays != 0 && ifExceed)
            return new ResultDto<Book>
            {
                Code = 3,
                Message =
                    $"You have some book(s) exceeded the borrow duration of {setting.BorrowDurationDays} days. Before borrowing new book(s), please return the exceeded book(s).",
                Data = null
            };

        var cbWithSameUserAndBook = await cb
            .Where(x => x.BookId == bookId)
            .FirstOrDefaultAsync();
        // 一个用户不能多次借阅同一种书
        if (cbWithSameUserAndBook != null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "It seems that you have already borrowed this book. You can't borrow it again.",
                Data = null
            };

        // 检查库存
        if (book.Inventory <= 0)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "This book is out of stock.",
                Data = null
            };

        // 所有检查均通过后，创建借阅记录
        var currentBorrowRecord = new CurrentBorrow
        {
            UserId = userId,
            BookId = bookId,
            BorrowDate = DateTime.Now
        };
        context.CurrentBorrows.Add(currentBorrowRecord);
        // 借阅时不增加借阅次数，归还时再增加
        book.Inventory--;
        await context.SaveChangesAsync();
        return new ResultDto<Book>
        {
            Code = 0,
            Message = "OK",
            Data = null
        };
    }

    [HttpPost("return")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> ReturnBook(int bookId)
    {
        var userId = GetUserId();

        var user = await context.Users.FindAsync(userId);
        var book = await context.Books.FindAsync(bookId);

        if (user == null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "User Not Found",
                Data = null
            };

        if (book == null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "You haven't borrowed this book yet.",
                Data = null
            };

        var currentBorrow = await context.CurrentBorrows
            .Where(x => x.UserId == userId && x.BookId == bookId)
            .FirstOrDefaultAsync();
        if (currentBorrow == null)
            return new ResultDto<Book>
            {
                Code = 1,
                Message = "It seems that you haven't borrowed this book.",
                Data = null
            };
        var borrowHistory = new BorrowHistory
        {
            UserId = userId,
            BookId = bookId,
            BorrowDate = currentBorrow.BorrowDate,
            ReturnDate = DateTime.Now
        };
        context.BorrowHistories.Add(borrowHistory);
        context.CurrentBorrows.Remove(currentBorrow);
        // 归还时增加借阅次数
        book.Inventory++;
        book.Borrowed++;
        await context.SaveChangesAsync();
        return new ResultDto<Book>
        {
            Code = 0,
            Message = "OK",
            Data = null
        };
    }

    [Authorize]
    [HttpPost("recommend")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Recommend>> RecommendBook(RecommendUserRequestDto requestDto)
    {
        var userId = GetUserId();
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return new ResultDto<Recommend>
            {
                Code = 1,
                Message = "User Not Found",
                Data = null
            };
        var recommend = new Recommend
        {
            UserId = userId,
            Title = requestDto.Title,
            Author = requestDto.Author ?? string.Empty,
            Publisher = requestDto.Publisher ?? string.Empty,
            Isbn = requestDto.Isbn ?? string.Empty,
            UserRemark = requestDto.Remark ?? string.Empty,
            IsProcessed = false,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.MinValue
        };
        context.Recommends.Add(recommend);
        await context.SaveChangesAsync();
        return new ResultDto<Recommend>
        {
            Code = 0,
            Message = "OK",
            Data = null
        };
    }

    [Authorize]
    [HttpGet("statistics")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<UserStatisticsDto>> GetUserStatistics()
    {
        var userId = GetUserId();
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return new ResultDto<UserStatisticsDto>
            {
                Code = 1,
                Message = "User Not Found",
                Data = null
            };
        var currentBorrowedBooks = await context.CurrentBorrows
            .Where(x => x.UserId == userId)
            .CountAsync();
        // 将当前用户的借阅历史记录全部取到内存中，然后进行统计。由于一个用户的借阅记录数量有限，内存的消耗应当是可以接受的。
        var borrowHistories = context.BorrowHistories
            .Where(x => x.UserId == userId)
            .Include(x => x.Book)
            .AsEnumerable();
        var enumerable = borrowHistories as BorrowHistory[] ?? borrowHistories.ToArray();
        var totalBorrowedBooks = enumerable.Length;
        var lastMonthBorrowedBooks = enumerable.Count(x => x.BorrowDate > DateTime.Now.AddMonths(-1));

        var past12Months = Enumerable.Range(0, 12)
            .Select(i => DateTime.Now.AddMonths(-i).ToString("yyyy-MM"))
            .Reverse();
        var monthlyBorrowedBooks = new Dictionary<string, int>();
        foreach (var month in past12Months)
        {
            var count = enumerable.Count(x => x.BorrowDate.ToString("yyyy-MM") == month);
            monthlyBorrowedBooks.Add(month, count);
        }

        var zhongTuClassification = new ZhongTuClassification();
        var borrowedBooksByClassification = enumerable
            .GroupBy(x => x.Book.Identifier[..1])
            .Select(x => new
            {
                Classification = zhongTuClassification.GetClassificationName(x.Key),
                Count = x.Count()
            })
            .OrderBy(x => x.Classification)
            .ToDictionary(g => g.Classification, g => g.Count);
        double averageBorrowDuration;
        if (totalBorrowedBooks == 0)
            averageBorrowDuration = 0;
        else
            averageBorrowDuration = enumerable.Average(x => (x.ReturnDate - x.BorrowDate).TotalDays);

        logger.LogInformation("User {UserId} statistics: {Info}", userId, new
        {
            totalBorrowedBooks,
            lastMonthBorrowedBooks,
            monthlyBorrowedBooks,
            borrowedBooksByClassification,
            averageBorrowDuration
        });

        return new ResultDto<UserStatisticsDto>
        {
            Code = 0,
            Message = "OK",
            Data = new UserStatisticsDto
            {
                TotalHistoryBorrowedBooks = totalBorrowedBooks,
                TotalCurrentBorrowedBooks = currentBorrowedBooks,
                LastMonthBorrowedBooks = lastMonthBorrowedBooks,
                MonthlyBorrowedBooks = monthlyBorrowedBooks,
                BorrowedBooksByClassification = borrowedBooksByClassification,
                AverageBorrowDuration = averageBorrowDuration.ToString("F2")
            }
        };
    }

    [Authorize]
    [HttpPost("comment")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<BookComment>> CommentBook(BookCommentRequestDto requestDto)
    {
        var userId = GetUserId();
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return new ResultDto<BookComment>
            {
                Code = 1,
                Message = "User Not Found",
                Data = null
            };
        var book = await context.Books.FindAsync(requestDto.BookId);
        if (book == null)
            return new ResultDto<BookComment>
            {
                Code = 1,
                Message = "Book Not Found",
                Data = null
            };
        var comment = new BookComment
        {
            UserId = userId,
            BookId = requestDto.BookId,
            RefCommentId = requestDto.RefCommentId,
            Content = requestDto.Content,
            CreateTime = DateTime.Now,
            User = user
        };
        context.BookComments.Add(comment);
        await context.SaveChangesAsync();

        return new ResultDto<BookComment>
        {
            Code = 0,
            Message = "OK",
            Data = null
        };
    }
}