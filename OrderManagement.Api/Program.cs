using OrderManagement.Api.Middleware;
using OrderManagement.Application;
using OrderManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Application = use cases (handlers). Infrastructure = EF Core + Oracle + repos.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Oracle")
    ?? throw new InvalidOperationException("Connection string 'Oracle' is missing."));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS for the React dev server (Vite runs on :5173, browser blocks
// cross-origin fetch without this). Browsers enforce the Same-Origin
// Policy: JS from localhost:5173 may not call localhost:8080 unless
// the API explicitly allows that origin. No cookies/auth here, so a
// simple origin allow-list is enough. Production SPAs served from the
// same origin wouldn't need CORS at all.
builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins("http://localhost:5173", "http://localhost:5174")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Must run before MapControllers so preflight (OPTIONS) requests
// are answered without hitting the controllers.
app.UseCors("Frontend");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
