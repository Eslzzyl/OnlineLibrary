using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using OnlineLibrary.Constant;
using OnlineLibrary.Model;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddSimpleConsole()
    .AddDebug();
builder.Host.UseSerilog((ctx, lc) => {
    lc.ReadFrom.Configuration(ctx.Configuration);
    lc.WriteTo.SQLite("./Data/Logs.db", tableName: "LogEvents");
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// CORS Configuration
builder.Services.AddCors(opt => {
    opt.AddDefaultPolicy(policy => {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
    opt.AddPolicy(name: "GetOnly", policy => {
        policy
            .AllowAnyHeader()
            .WithMethods("GET")
            .AllowAnyOrigin();
    });
});

builder.Services.AddIdentity<ApiUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
                options.DefaultScheme =
                    options.DefaultSignInScheme =
                        options.DefaultSignOutScheme =
                            JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(opt => {
    opt.UseSqlite("Filename=./Data/OnlineLibrary.db");
    // opt.EnableSensitiveDataLogging();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/error",
    [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
    () => Results.Problem());
app.MapGet("/auth/test/1",
    [Authorize] [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
    () => Results.Ok("You are authorized, User!"));
app.MapGet("/auth/test/2",
    [Authorize(Roles = RoleNames.Moderator)] [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
    () => Results.Ok("You are authorized, Moderator!"));
app.MapGet("/auth/test/3",
    [Authorize(Roles = RoleNames.Admin)] [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
    () => Results.Ok("You are authorized, Admin!"));

app.MapControllers();

app.Run();
