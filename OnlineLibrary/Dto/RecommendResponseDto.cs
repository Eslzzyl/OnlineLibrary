namespace OnlineLibrary.Dto;

public record RecommendResponseDto
{
    public int Id { get; init; }
    
    public string Title { get; init; } = default!;
    
    public string? Author { get; init; }
    
    public string? Publisher { get; init; }
    
    public string? Isbn { get; init; }
    
    public string? UserRemark { get; init; }
    
    public string IsProcessed { get; init; } = default!;
    
    public string? AdminRemark { get; init; }
    
    public string CreateTime { get; init; } = default!;
    
    public string UpdateTime { get; init; } = default!;
    
    public string UserName { get; init; } = default!;

    public string? AdminName { get; init; }
}