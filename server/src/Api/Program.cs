using System.Text;

using Api.Middleware.Exceptions;

using Application;

using Domain.Entities;

using Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Persistence;
using Persistence.DatabaseContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServer(builder.Configuration);

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, // Reduces token lifetime tolerance

        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
    };
});

// Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
    // Strong password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Security lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    // User validation
    options.User.RequireUniqueEmail = true;
});

// Exception Handling Services
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<EntityNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<DuplicateEntityExceptionHandler>();
builder.Services.AddExceptionHandler<UpdateUserExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add and configure Swagger middleware
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Omnipulse API",
        Version = "v1",
        Description = "A comprehensive fleet management system API.",
        Contact = new OpenApiContact
        {
            Name = "OmniDevs",
            Email = builder.Configuration["Contact:Email"] ?? "Nicholassaw2004@gmail.com"
        }
    });

    // Group by controller/tag
    c.TagActionsBy(api => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"]]);

    // Include XML documentation comments from all XML files (excluding test/coverage files)
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml")
        .Where(file =>
            !file.Contains("Test") &&
            !file.Contains("coverage") &&
            !file.Contains("TestResults"))
        .ToList();

    foreach (var xmlFile in xmlFiles)
    {
        c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }
});

// Add CORS to allow frontend requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "https://localhost:3000", // Next.js - Default port
                "http://localhost:3000",  // Next.js - Default port
                "https://omnipulse-frontend.wonderfulsky-7bfd34c0.australiaeast.azurecontainerapps.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );

    // Use only in development/testing
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Omnipulse API");
        c.RoutePrefix = "swagger";
    });

    app.UseCors("AllowAll");

    // Automatically apply migrations. Create the database if it does not exist
    using var scope = app.Services.CreateScope();
    var omnipulseDbContext = scope.ServiceProvider.GetRequiredService<OmnipulseDatabaseContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await omnipulseDbContext.Database.EnsureCreatedAsync(); // Triggers UseAsyncSeeding()
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;