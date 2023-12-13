namespace OnlineLibrary.Dto;

using System.ComponentModel.DataAnnotations;

public record LoginRequestDto
{
    public required string Account { get; set; }
    
    public required string Password { get; set; }
}