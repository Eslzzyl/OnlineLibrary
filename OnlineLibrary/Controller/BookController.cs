namespace OnlineLibrary.Controller;

using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Constant;
using Dto;
using Model;

[Route("/book")]
[ApiController]
public class BookController(ILogger<BookController> logger, ApplicationDbContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<Book[]>> GetBooks(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        var query = context.Books.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Title.Contains(filterQuery) || 
                x.Author.Contains(filterQuery) ||
                x.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<Book>)query
            .OrderBy($"{sortColumn} {sortOrder}")
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
        return new ResultDto<Book[]>() {
            Code = 0,
            Message = "OK",
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }
    
    [Authorize(Roles = RoleNames.Moderator)]
    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> AddBook(BookDto bookDto) {
        var book = new Book() {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Publisher = bookDto.Publisher,
            PublishedDate = bookDto.PublishedDate,
            Identifier = bookDto.Identifier,
            InboundDate = bookDto.InboundDate,
            Inventory = bookDto.Inventory,
            Borrowed = 0,
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        return new ResultDto<Book>() {
            Code = 0,
            Message = "OK",
            Data = null,
        };
    }
    
    [Authorize(Roles = RoleNames.Moderator)]
    [HttpPut]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> UpdateBook(BookDto bookDto) {
        var book = await context.Books.FindAsync(bookDto.Id);
        if (book == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "Not Found",
                Data = null,
            };
        }
        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Publisher = bookDto.Publisher;
        book.PublishedDate = bookDto.PublishedDate;
        book.Identifier = bookDto.Identifier;
        book.InboundDate = bookDto.InboundDate;
        book.Inventory = bookDto.Inventory;
        await context.SaveChangesAsync();
        return new ResultDto<Book>() {
            Code = 0,
            Message = "OK",
            Data = null,
        };
    }
    
    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<Book>> DeleteBook(int id) {
        var book = await context.Books.FindAsync(id);
        if (book == null) {
            return new ResultDto<Book>() {
                Code = 1,
                Message = "Not Found",
                Data = null,
            };
        }
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return new ResultDto<Book>() {
            Code = 0,
            Message = "OK",
            Data = null,
        };
    }
    
    [Authorize(Roles = RoleNames.Moderator)]
    [HttpGet("currentborrow")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<BorrowDto[]>> GetCurrentBorrow(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        var query = context.CurrentBorrows
            .Include(x => x.Book)
            .Include(x => x.User)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) || 
                x.Book.Author.Contains(filterQuery) ||
                x.Book.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<CurrentBorrow>)query
            .OrderBy($"Book.{sortColumn} {sortOrder}")
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.BookId,
            UserName = x.User.UserName,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate,
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
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<ResultDto<BorrowDto[]>> GetBorrowHistory(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        var query = context.BorrowHistories
            .Include(x => x.Book)
            .Include(x => x.User)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Book.Title.Contains(filterQuery) || 
                x.Book.Author.Contains(filterQuery) ||
                x.Book.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<BorrowHistory>)query
            .OrderBy($"Book.{sortColumn} {sortOrder}")
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.BookId,
            UserName = x.User.UserName,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate,
            ReturnDate = x.ReturnDate,
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
}