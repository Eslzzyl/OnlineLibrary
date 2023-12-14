using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Dto;
using OnlineLibrary.Model.DatabaseContext;

namespace OnlineLibrary.Controller;

[Route("bookcomment")]
[ApiController]
public class BookCommentController(
    ILogger<UserController> logger,
    ApplicationDbContext context)
    : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<BookCommentResponseDto>> GetBookComments(int bookId) {
        var book = await context.Books.FindAsync(bookId);
        if (book == null) {
            return new ResultDto<BookCommentResponseDto>() {
                Code = 404,
                Message = "Book not found.",
                Data = null
            };
        }

        var commentsList = await context.BookComments
            .Where(x => x.BookId == bookId)
            .Include(x => x.User)
            .OrderBy(x => x.CreateTime)
            .ToListAsync();

        uint index = 0;
        var dict = new Dictionary<int, uint>();
        foreach (var comment in commentsList) {
            dict[comment.Id] = index++;
        }
        index = 0;
        var comments = commentsList
            .Select(comment => new CommentUnit {
                Index = index++,
                Id = comment.Id,
                UserName = comment.User.UserName,
                UserAvatar = comment.User.Avatar,
                RefCommentIndex = comment.RefCommentId == 0 ? 0 : dict[comment.RefCommentId],
                Content = comment.Content,
                CreateTime = comment.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();

        return new ResultDto<BookCommentResponseDto>() {
            Code = 0,
            Message = "OK",
            Data = new BookCommentResponseDto() {
                BookId = bookId,
                Comments = comments
            }
        };
    }
}