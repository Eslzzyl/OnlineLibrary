namespace OnlineLibrary.Dto;

public record BookDto
{
    public int Id { get; private set; }
    
    public string Title { get; private set; } = default!;
    
    public string Author { get; private set; } = default!;
    
    public string Publisher { get; private set; } = default!;
    
    public string PublishedDate { get; private set; } = default!;
    
    public string Identifier { get; private set; } = default!;
    
    public DateTime InboundDate { get; private set; }
    
    public uint Inventory { get; private set; }
}