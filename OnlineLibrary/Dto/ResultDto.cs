namespace OnlineLibrary.Dto;

public record ResultDto<T>
{
    public T? Data { get; set; } = default!;

    public required int Code { get; set; } = 0;

    public required string Message { get; set; } = string.Empty;

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int RecordCount { get; set; }
}