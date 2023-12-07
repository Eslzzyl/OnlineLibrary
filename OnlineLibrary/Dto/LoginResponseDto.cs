namespace OnlineLibrary.Dto;

using System.ComponentModel.DataAnnotations;

public record LoginResponseDto
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
}