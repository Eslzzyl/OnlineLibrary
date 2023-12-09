using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using System.Linq.Dynamic.Core;

namespace OnlineLibrary.Controller;

[Route("[controller]")]
[ApiController]
public class LogController(ApplicationDbContext context,
    LogsDbContext logsDbContext,
    ILogger<LogController> logger)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ResultDto<LogEvent[]>> Get(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Id",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        if (pageIndex < 0) {
            pageIndex = 0;
        }
        var query = logsDbContext.LogEvents.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Level.Contains(filterQuery) ||
                x.Exception.Contains(filterQuery) ||
                x.RenderedMessage.Contains(filterQuery) ||
                x.Properties.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<LogEvent>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        return new ResultDto<LogEvent[]>() {
            Code = 0,
            Message = "OK",
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }
}