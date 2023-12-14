using CsvHelper.Configuration.Attributes;

namespace OnlineLibrary.Model.Csv;

public class BookRecord
{
    // Title,Author,Identifier,Inventory,Count,Rate,Publisher,PublishedDate

    [Name("Title")]
    public string Title { get; set; } = default!;

    [Name("Author")]
    public string Author { get; set; } = default!;

    [Name("Identifier")]
    public string Identifier { get; set; } = default!;

    [Name("Inventory")]
    public uint Inventory { get; set; }

    [Name("Count")]
    public uint Count { get; set; }

    [Name("Rate")]
    public double Rate { get; set; }

    [Name("Publisher")]
    public string Publisher { get; set; } = default!;

    [Name("PublishedDate")]
    public string PublishedDate { get; set; } = default!;
}