namespace OnlineLibrary.Dto;

public class RecommendUserRequestDto
{
    public string Title { get; init; } = default!;
    
    public string? Author { get; init; }
    
    public string? Publisher { get; init; }
    
    public string? Isbn { get; init; }
    
    public string? Remark { get; init; }
}