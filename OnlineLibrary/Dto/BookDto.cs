namespace OnlineLibrary.Dto;

public record BookDto
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string Author { get; set; } = default!;

    public string Publisher { get; set; } = default!;

    public string PublishedDate { get; set; } = default!;

    public string Identifier { get; set; } = default!;

    public uint Inventory { get; set; }
}