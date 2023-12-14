using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Dto;
using OnlineLibrary.Model;
using OnlineLibrary.Model.DatabaseContext;
using Serilog;
using System.Linq.Dynamic.Core;

namespace OnlineLibrary.Controller;

[Route("[controller]")]
[ApiController]
public class LogController(
    ApplicationDbContext context,
    LogsDbContext logsDbContext,
    ILogger<LogController> logger,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ResultDto<LogEvent[]>> Get(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Id",
        string? sortOrder = "ASC",
        string? filterQuery = null)
    {
        if (pageIndex < 0)
            pageIndex = 0;
        var query = logsDbContext.LogEvents.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery))
            query = query.Where(
                x => x.Level.Contains(filterQuery) ||
                    x.Exception.Contains(filterQuery) ||
                    x.RenderedMessage.Contains(filterQuery) ||
                    x.Properties.Contains(filterQuery));
        var recordCount = await query.CountAsync();
        query = (IQueryable<LogEvent>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        return new ResultDto<LogEvent[]>
        {
            Code = 0,
            Message = "OK",
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount
        };
    }

    [HttpDelete]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ResultDto<LogEvent>> Delete()
    {
        // Stop Serilog logging and flush all cached logs
        await Log.CloseAndFlushAsync();

        // Delete all logs from database
        logsDbContext.LogEvents.RemoveRange(logsDbContext.LogEvents);
        await logsDbContext.SaveChangesAsync();
        logsDbContext.RunVacuum();

        // Restart Serilog logging
        var projectRootPath = Directory.GetCurrentDirectory();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.SQLite(Path.Combine(projectRootPath, "./Data/Logs.db"), "LogEvents")
            .CreateLogger();

        return new ResultDto<LogEvent>
        {
            Data = null,
            Code = 0,
            Message = "OK"
        };
    }
}