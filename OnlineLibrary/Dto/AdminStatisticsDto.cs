namespace OnlineLibrary.Dto;

public record AdminStatisticsDto
{
    public required int TotalHistoryBorrowedBooks { get; set; }

    public required int TotalCurrentBorrowedBooks { get; set; }

    public required int TotalUsers { get; set; }

    public required Dictionary<string, int> MonthlyBorrowedBooks { get; set; }

    public required Dictionary<string, int> BorrowedBooksByClassification { get; set; }

    public required string AverageBorrowDuration { get; set; }

    public required int UnhandledRecommends { get; set; }
}