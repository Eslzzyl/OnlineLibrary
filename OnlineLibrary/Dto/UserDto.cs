namespace OnlineLibrary.Dto;

public record UserDto
{
    public string Id { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public ICollection<string>? Roles { get; set; }
    
    
}