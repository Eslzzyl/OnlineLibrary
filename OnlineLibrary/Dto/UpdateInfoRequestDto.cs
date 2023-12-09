namespace OnlineLibrary.Dto;

public record UpdateInfoRequestDto
{
    public string? Email { get; set; } = default!;

    public string? PhoneNumber { get; set; } = default!;
    
    public string? Avatar { get; set; } = default!;
    
    public string? Password { get; set; } = default!;
}