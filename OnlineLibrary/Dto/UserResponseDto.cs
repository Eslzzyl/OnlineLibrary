namespace OnlineLibrary.Dto;

public record UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    public ICollection<string> Roles { get; set; } = new List<string>();
    
    public string Avatar { get; set; } = string.Empty;
}