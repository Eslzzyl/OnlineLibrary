namespace OnlineLibrary.Dto;

public record BorrowDto
{
    public int Id { get; set; }

    public string? UserName { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string Publisher { get; set; } = string.Empty;
    
    public DateTime BorrowDate { get; set; }
    
    public DateTime? ReturnDate { get; set; }
}