using Microsoft.EntityFrameworkCore;
using FullStackAssessment.Api.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the DbContext using the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Pre-configuring CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Default Angular port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseAuthorization();
app.MapControllers();
app.Run();