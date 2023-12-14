namespace OnlineLibrary.Dto;

public class BookCommentRequestDto
{
    public required int BookId { get; set; }
    
    public required int RefCommentId { get; set; }
    
    public required string Content { get; set; } = default!;
}