namespace OnlineLibrary.Dto;

public class CommentUnit
{
    public required uint Index { get; init; }

    public required int Id { get; init; }

    public required string UserName { get; set; } = default!;

    public required string UserAvatar { get; set; } = default!;

    public uint RefCommentIndex { get; set; }

    public string Content { get; set; } = default!;

    public string CreateTime { get; set; } = default!;
}

public class BookCommentResponseDto
{
    public required int BookId { get; set; }

    public required List<CommentUnit> Comments { get; set; } = default!;
}