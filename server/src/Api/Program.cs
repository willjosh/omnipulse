using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
            Email = "Nicholassaw2004@gmail.com"
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
builder.Services.AddCors(
    options => options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Omnipulse API");
        c.RoutePrefix = "swagger";
    });
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

app.Run();