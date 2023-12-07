namespace OnlineLibrary.Dto;

using System.ComponentModel.DataAnnotations;

public record RegisterRequestDto
{
    [Required]
    [MaxLength(255)]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}