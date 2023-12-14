using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Constant;
using OnlineLibrary.Model;
using OnlineLibrary.Model.Csv;
using OnlineLibrary.Model.DatabaseContext;
using System.Globalization;

namespace OnlineLibrary.Controller;

[Authorize(Roles = RoleNames.Admin)]
[Route("[controller]/[action]")]
[ApiController]
public class SeedController(
    ApplicationDbContext context,
    IWebHostEnvironment env,
    ILogger<SeedController> logger,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApiUser> userManager)
    : ControllerBase
{
    [HttpPut]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> BookData()
    {
        // SETUP
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            MissingFieldFound = null,
            BadDataFound = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        using var reader = new StreamReader(env.ContentRootPath + "/Data/Books.csv");
        using var csv = new CsvReader(reader, config);
        var existingBooks = await context.Books.ToDictionaryAsync(book => book.Identifier);

        // EXECUTE
        var records = csv.GetRecords<BookRecord>();
        foreach (var record in records)
        {
            if (existingBooks.ContainsKey(record.Identifier))
            {
                logger.LogInformation("Book {Title} already exists.", record.Title);
                continue;
            }
            var book = new Book
            {
                Title = record.Title,
                Author = record.Author,
                Publisher = record.Publisher,
                PublishedDate = record.PublishedDate,
                Identifier = record.Identifier,
                Inventory = record.Inventory,
                Borrowed = record.Count,
                InboundDate = DateTime.Now
            };
            await context.Books.AddAsync(book);
            logger.LogInformation("Book {Title} added.", record.Title);
        }
        await context.SaveChangesAsync();

        return new JsonResult(new
        {
            books = context.Books.Count()
        });
    }

    [HttpPut]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> SettingsData()
    {
        var settings = await context.Settings.FirstOrDefaultAsync();
        if (settings != null)
            return new JsonResult(new
            {
                addedSettings = 0
            });
        var setting = new Setting
        {
            BorrowLimit = 20,
            BorrowDurationDays = 30
        };
        await context.Settings.AddAsync(setting);
        await context.SaveChangesAsync();
        return new JsonResult(new
        {
            addedSettings = 2
        });
    }

    [HttpPut]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> AuthData()
    {
        var usersCreated = 0;
        var rolesCreated = 0;
        var usersAddedToRoles = 0;

        // 添加用户
        var user = await userManager.FindByNameAsync("TestUser");
        if (user == null)
        {
            user = new ApiUser
            {
                UserName = "TestUser",
                Email = "testuser@online-library.org"
            };
            await userManager.CreateAsync(user, "123456");
            usersCreated++;
        }

        var moderator = await userManager.FindByNameAsync("TestModerator");
        if (moderator == null)
        {
            moderator = new ApiUser
            {
                UserName = "TestModerator",
                Email = "testmoderator@online-library.org"
            };
            await userManager.CreateAsync(moderator, "123456");
            usersCreated++;
        }

        var admin = await userManager.FindByNameAsync("TestAdmin");
        if (admin == null)
        {
            admin = new ApiUser
            {
                UserName = "TestAdmin",
                Email = "testadmin@online-library.org"
            };
            await userManager.CreateAsync(admin, "123456");
            usersCreated++;
        }

        var deleted = await userManager.FindByNameAsync("DeletedUser");
        if (deleted == null)
        {
            deleted = new ApiUser
            {
                UserName = "DeletedUser",
                Email = "deleted@online-library.org"
            };
            await userManager.CreateAsync(deleted, "123456");
            usersCreated++;
        }

        // 添加角色，一共有3种角色

        if (!await roleManager.RoleExistsAsync(RoleNames.User))
        {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.User));
            rolesCreated++;
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.Moderator))
        {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.Moderator));
            rolesCreated++;
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.Admin))
        {
            await roleManager.CreateAsync(
                new IdentityRole(RoleNames.Admin));
            rolesCreated++;
        }

        // 为3种角色对应的测试用户添加用户组，如果用户不存在就什么也不做
        var testUser = await userManager.FindByNameAsync("TestUser");
        if (testUser != null && !await userManager.IsInRoleAsync(testUser, RoleNames.User))
        {
            await userManager.AddToRoleAsync(testUser, RoleNames.User);
            usersAddedToRoles++;
        }

        var testModerator = await userManager.FindByNameAsync("TestModerator");
        if (testModerator != null)
        {
            if (!await userManager.IsInRoleAsync(testModerator, RoleNames.Moderator))
                await userManager.AddToRoleAsync(testModerator, RoleNames.Moderator);
            if (!await userManager.IsInRoleAsync(testModerator, RoleNames.User))
                await userManager.AddToRoleAsync(testModerator, RoleNames.User);
            usersAddedToRoles++;
        }

        var testAdmin = await userManager.FindByNameAsync("TestAdmin");
        if (testAdmin != null)
        {
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.Admin))
                await userManager.AddToRoleAsync(testAdmin, RoleNames.Admin);
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.Moderator))
                await userManager.AddToRoleAsync(testAdmin, RoleNames.Moderator);
            if (!await userManager.IsInRoleAsync(testAdmin, RoleNames.User))
                await userManager.AddToRoleAsync(testAdmin, RoleNames.User);
            usersAddedToRoles++;
        }

        var deletedUser = await userManager.FindByNameAsync("DeletedUser");
        if (deletedUser != null)
        {
            if (!await userManager.IsInRoleAsync(deletedUser, RoleNames.User))
                await userManager.AddToRoleAsync(deletedUser, RoleNames.User);
            usersAddedToRoles++;
        }

        return new JsonResult(new
        {
            UsersCreated = usersCreated,
            RolesCreated = rolesCreated,
            UsersAddedToRoles = usersAddedToRoles
        });
    }
}