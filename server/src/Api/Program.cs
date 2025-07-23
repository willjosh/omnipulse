using Application;

using Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Persistence;
using Persistence.DatabaseContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServer(builder.Configuration);

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
                "https://localhost:3001", // Next.js - Alternate port
                "http://localhost:3001"   // Next.js - Alternate port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // Can include credentials like cookies, HTTP authentication, or Authorization headers with cross-origin requests
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
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Automatically apply migrations. Create the database if it does not exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OmnipulseDatabaseContext>();
    db.Database.Migrate();
}

app.Run();