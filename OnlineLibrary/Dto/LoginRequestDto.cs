namespace OnlineLibrary.Dto;

public record LoginRequestDto
{
    public required string Account { get; set; }

    public required string Password { get; set; }
}