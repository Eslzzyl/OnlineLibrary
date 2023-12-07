namespace OnlineLibrary.Dto;

using System.ComponentModel.DataAnnotations;

public record LoginRequestDto
{
    [Required]
    [MaxLength(255)]
    public string? Account { get; set; }

    [Required]
    public string? Password { get; set; }
}