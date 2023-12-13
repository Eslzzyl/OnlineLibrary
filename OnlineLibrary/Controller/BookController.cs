using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using System.Linq.Dynamic.Core;

namespace OnlineLibrary.Controller;

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
        if (pageIndex < 0) {
            pageIndex = 0;
        }
        var query = context.Books.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Title.Contains(filterQuery) || 
                x.Author.Contains(filterQuery) ||
                x.Publisher.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<Book>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        
        logger.LogInformation(
            "FilterQuery: {FilterQuery}, pageIndex={PageIndex}, pageSize={PageSize}, sortColumn={SortColumn}, sortIndex={SortIndex}.",
            filterQuery, pageIndex, pageSize, sortColumn, sortOrder);
        logger.LogInformation("Got {Count} books", await query.CountAsync());
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

        logger.LogInformation("Added book {Book}", book);
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
        
        logger.LogInformation("Updated book {Book}", book);
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
        // 同时删除与之相关联的借阅记录
        var currentBorrows = await context.CurrentBorrows
            .Where(x => x.BookId == id)
            .ToArrayAsync();
        context.CurrentBorrows.RemoveRange(currentBorrows);
        var borrowHistories = await context.BorrowHistories
            .Where(x => x.BookId == id)
            .ToArrayAsync();
        context.BorrowHistories.RemoveRange(borrowHistories);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Deleted book {Book}", book);
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
        if (pageIndex < 0) {
            pageIndex = 0;
        }
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
        query = (IQueryable<CurrentBorrow>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.BookId,
            UserName = x.User.UserName,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            ReturnDate = null,
        }).ToArrayAsync();
        
        logger.LogInformation("Got {Count} current borrows", await query.CountAsync());
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
        if (pageIndex < 0) {
            pageIndex = 0;
        }
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
        query = (IQueryable<BorrowHistory>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"Book.{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new BorrowDto() {
            Id = x.BookId,
            UserName = x.User.UserName,
            Title = x.Book.Title,
            Author = x.Book.Author,
            Publisher = x.Book.Publisher,
            BorrowDate = x.BorrowDate.Date.ToString("yyyy-MM-dd"),
            ReturnDate = x.ReturnDate.Date.ToString("yyyy-MM-dd"),
            BorrowDuration = (x.ReturnDate - x.BorrowDate).TotalDays.ToString("F2"),
        }).ToArrayAsync();
        
        logger.LogInformation("Got {Count} borrow histories", await query.CountAsync());
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