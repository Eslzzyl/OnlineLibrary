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
public class RecommendController(ApplicationDbContext context,
    LogsDbContext logsDbContext,
    ILogger<LogController> logger)
    : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ResultDto<RecommendResponseDto[]>> GetRecommend(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Title",
        string? sortOrder = "ASC",
        string? filterQuery = null) {
        var query = context.Recommends
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(filterQuery)) {
            query = query.Where(
                x => x.Title.Contains(filterQuery) ||
                    x.Author.Contains(filterQuery) ||
                    x.Publisher.Contains(filterQuery) ||
                    x.Isbn.Contains(filterQuery) ||
                    x.UserRemark.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = (IQueryable<Recommend>)DynamicQueryableExtensions.Skip(query
                .OrderBy($"{sortColumn} {sortOrder}"), pageIndex * pageSize)
            .Take(pageSize);
        var dto = await query.Select(x => new RecommendResponseDto() {
            Id = x.Id,
            Title = x.Title,
            Author = x.Author,
            Publisher = x.Publisher,
            Isbn = x.Isbn,
            UserRemark = x.UserRemark,
            IsProcessed = x.IsProcessed ? "是" : "否",
            AdminRemark = x.AdminRemark,
            CreateTime = x.CreateTime.ToString("yyyy-MM-dd HH:mm"),
            UpdateTime = (x.UpdateTime != DateTime.MinValue) ? x.UpdateTime.ToString("yyyy-MM-dd HH:mm") : "尚未处理",
            UserName = context.Users.Where(u => u.Id == x.UserId).Select(u => u.UserName).FirstOrDefault(),
            AdminName = context.Users.Where(u => u.Id == x.AdminId).Select(u => u.UserName).FirstOrDefault(),
        }).ToArrayAsync();
        
        logger.LogInformation(
            "FilterQuery: {FilterQuery}, pageIndex={PageIndex}, pageSize={PageSize}, sortColumn={SortColumn}, sortIndex={SortIndex}.",
            filterQuery, pageIndex, pageSize, sortColumn, sortOrder);
        logger.LogInformation("Got {Count} recommends", await query.CountAsync());
        return new ResultDto<RecommendResponseDto[]>() {
            Code = 0,
            Message = "OK",
            Data = dto,
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
        };
    }
}