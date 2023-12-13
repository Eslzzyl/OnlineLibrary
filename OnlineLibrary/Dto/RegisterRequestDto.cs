namespace OnlineLibrary.Dto;

using System.ComponentModel.DataAnnotations;

public record RegisterRequestDto
{
    public required string UserName { get; set; }
    
    public required string Password { get; set; }
}