using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Model;
using OnlineLibrary.Model.Csv;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OnlineLibrary.Constant;

namespace OnlineLibrary.Controller;

[Authorize(Roles = RoleNames.Admin)]
[Route("[controller]/[action]")]
[ApiController]
public class SeedController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<SeedController> logger,
        RoleManager<IdentityRole> roleManager, UserManager<ApiUser> userManager)
    : ControllerBase
{
    [HttpPut]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> BookData() {
        // SETUP
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
            HasHeaderRecord = true,
            Delimiter = ",",
            MissingFieldFound = null,
            BadDataFound = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
        };
        
        using var reader = new StreamReader(env.ContentRootPath + "/Data/Books.csv");
        using var csv = new CsvReader(reader, config);
        var existingBooks = await context.Books.ToDictionaryAsync(book => book.Identifier);

        // EXECUTE
        var records = csv.GetRecords<BookRecord>();
        foreach (var record in records) {
            if (existingBooks.ContainsKey(record.Identifier)) {
                logger.LogInformation($"Book {record.Title} already exists.");
                continue;
            }
            var book = new Book() {
                Title = record.Title,
                Author = record.Author,
                Publisher = record.Publisher,
                PublishedDate = record.PublishedDate,
                Identifier = record.Identifier,
                Inventory = record.Inventory,
                Borrowed = record.Count,
                InboundDate = DateTime.Now,
            };
            await context.Books.AddAsync(book);
            logger.LogInformation($"Book {record.Title} added.");
        }
        await context.SaveChangesAsync();

        return new JsonResult(new {
            books = context.Books.Count()
        });
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> AuthData() {
        var rolesCreated = 0;
        var usersAddedToRoles = 0;

        // 添加角色，一共有3种角色
        
        if (!await roleManager.RoleExistsAsync(RoleNames.User)) {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.User));
            rolesCreated++;
        }
        
        if (!await roleManager.RoleExistsAsync(RoleNames.Moderator)) {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.Moderator));
            rolesCreated++;
        }
        
        if (!await roleManager.RoleExistsAsync(RoleNames.Admin)) {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.Admin));
            rolesCreated++;
        }
        
        // 为3种角色对应的测试用户添加用户组，如果用户不存在就什么也不做
        var testUser = await userManager.FindByNameAsync("TestUser");
        if (testUser != null && !await userManager.IsInRoleAsync(testUser, RoleNames.User)) {
            await userManager.AddToRoleAsync(testUser, RoleNames.User);
            usersAddedToRoles++;
        }
        
        var testModerator = await userManager.FindByNameAsync("TestModerator");
        if (testModerator != null) {
            if (!await userManager.IsInRoleAsync(testModerator, RoleNames.Moderator)) {
                await userManager.AddToRoleAsync(testModerator, RoleNames.Moderator);
            }
            if (!await userManager.IsInRoleAsync(testModerator, RoleNames.User)) {
                await userManager.AddToRoleAsync(testModerator, RoleNames.User);
            }
            usersAddedToRoles++;
        }
        
        var testAdmin = await userManager.FindByNameAsync("TestAdmin");
        if (testAdmin != null) {
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.Admin)) {
                await userManager.AddToRoleAsync(testAdmin, RoleNames.Admin);
            }
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.Moderator)) {
                await userManager.AddToRoleAsync(testAdmin, RoleNames.Moderator);
            }
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.User)) {
                await userManager.AddToRoleAsync(testAdmin, RoleNames.User);
            }
            usersAddedToRoles++;
        }
        
        return new JsonResult(new {
            RolesCreated = rolesCreated,
            UsersCreated = usersAddedToRoles,
        });
    }
}