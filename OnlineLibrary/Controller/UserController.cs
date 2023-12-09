using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
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
    private string GetUserId() {
        // Get user id from token
        string userId;
        if (httpContextAccessor.HttpContext != null) {
            var claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null) {
                userId = claim.Value;
            }
            else {
                throw new Exception();
            }
        }
        else {
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
        string? filterQuery = null) {
        var userId = GetUserId();
        var query = context.CurrentBorrows
            .Include(x => x.Book)
            .AsQueryable();
        query = query.Where(x => x.UserId == userId);
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) ||
                    x.Book.Author.Contains(filterQuery) ||
                    x.Book.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<CurrentBorrow>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.Book.Id,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            ReturnDate = null,
        }).ToArrayAsync();
        return new ResultDto<BorrowDto[]>() {
            Code = 0,
            Message = "OK",
            Data = dto,
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
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
        string? filterQuery = null) {
        var userId = GetUserId();
        var query = context.BorrowHistories
            .Include(x => x.Book)
            .AsQueryable();
        query = query.Where(x => x.UserId == userId);
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) ||
                    x.Book.Author.Contains(filterQuery) ||
                    x.Book.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<BorrowHistory>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.Book.Id,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            ReturnDate = x.ReturnDate.Date.ToString("yyyy-MM-dd"),
            BorrowDuration = ((int)(x.ReturnDate - x.BorrowDate).TotalDays).ToString(),
        }).ToArrayAsync();
        return new ResultDto<BorrowDto[]>() {
            Code = 0,
            Message = "OK",
            Data = dto,
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }

    [HttpPost("borrow")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> BorrowBook(int bookId) {
        var userId = GetUserId();

        var user = await context.Users.FindAsync(userId);
        var book = await context.Books.FindAsync(bookId);

        if (user == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "User Not Found",
                Data = null,
            };
        }
        
        if (book == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "Book Not Found",
                Data = null,
            };
        }

        // 检查是否达到最大借书量
        var setting = await context.Settings.FirstOrDefaultAsync();
        var cb = context.CurrentBorrows.Where(x => x.UserId == userId);
        if (setting.BorrowLimit != 0 && cb.Count() >= setting.BorrowLimit) {
            return new ResultDto<Book>() {
                Code = 2,
                Message = $"You have achieved the borrow limit of {setting.BorrowLimit}",
                Data = null,
            };
        }
        
        // 检查用户当前借书表中是否有超期
        // var ifExceed = cb.
        
        var cbWithSameUserAndBook = await cb
            .Where(x => x.BookId == bookId)
            .FirstOrDefaultAsync();
        // 一个用户不能多次借阅同一种书
        if (cbWithSameUserAndBook != null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "It seems that you have already borrowed this book. You can't borrow it again.",
                Data = null,
            };
        }

        if (book.Inventory <= 0) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "This book is out of stock.",
                Data = null,
            };
        }
        var currentBorrowRecord = new CurrentBorrow() {
            UserId = userId,
            BookId = bookId,
            BorrowDate = DateTime.Now,
        };
        context.CurrentBorrows.Add(currentBorrowRecord);
        // 借阅时不增加借阅次数，归还时再增加
        book.Inventory--;
        await context.SaveChangesAsync();
        return new ResultDto<Book>() {
            Code = 0,
            Message = "OK",
            Data = null,
        };
    }

    [HttpPost("return")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> ReturnBook(int bookId) {
        var userId = GetUserId();

        var user = await context.Users.FindAsync(userId);
        var book = await context.Books.FindAsync(bookId);

        if (user == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "User Not Found",
                Data = null,
            };
        }

        if (book == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "You haven't borrowed this book yet.",
                Data = null,
            };
        }

        var currentBorrow = await context.CurrentBorrows
            .Where(x => x.UserId == userId && x.BookId == bookId)
            .FirstOrDefaultAsync();
        if (currentBorrow == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "It seems that you haven't borrowed this book.",
                Data = null,
            };
        }
        var borrowHistory = new BorrowHistory() {
            UserId = userId,
            BookId = bookId,
            BorrowDate = currentBorrow.BorrowDate,
            ReturnDate = DateTime.Now,
        };
        context.BorrowHistories.Add(borrowHistory);
        context.CurrentBorrows.Remove(currentBorrow);
        // 归还时增加借阅次数
        book.Inventory++;
        book.Borrowed++;
        await context.SaveChangesAsync();
        return new ResultDto<Book>() {
            Code = 0,
            Message = "OK",
            Data = null,
        };
    }
}