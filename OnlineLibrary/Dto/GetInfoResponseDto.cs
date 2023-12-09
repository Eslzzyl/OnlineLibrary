namespace OnlineLibrary.Dto;

public record GetInfoResponseDto
{
    public string Email { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;
    
    public string Avatar { get; set; } = default!;
}