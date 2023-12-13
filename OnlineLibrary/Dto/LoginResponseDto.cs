namespace OnlineLibrary.Dto;

public record LoginResponseDto
{
    public required string UserName { get; set; }
    public required string Token { get; set; }
    public required string Role { get; set; }
    public required string Avatar { get; set; }
}