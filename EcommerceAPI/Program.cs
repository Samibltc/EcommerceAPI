using EcommerceAPI.Configuration;
using EcommerceAPI.Business.DependencyInjection;
using EcommerceAPI.DataAccess.DependencyInjection;
using EcommerceAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// API layer (Controllers, ValidationFilter, FluentValidation scanning)
builder.Services.AddApiServices();

// Business & DataAccess registrations
builder.Services.AddBusinessServices();
builder.Services.AddDataAccessServices(builder.Configuration);

// Swagger/OpenAPI
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
