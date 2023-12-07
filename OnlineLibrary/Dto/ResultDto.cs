namespace OnlineLibrary.Dto;

public record ResultDto<T>
{
    public T? Data { get; set; } = default!;
    
    public int Code { get; set; } = 0;
    
    public string Message { get; set; } = string.Empty;
    
    public int? PageIndex { get; set; }
    
    public int? PageSize { get; set; }
    
    public int? RecordCount { get; set; }
}