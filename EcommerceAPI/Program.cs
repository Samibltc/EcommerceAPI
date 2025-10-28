using EcommerceAPI.Configuration;
using EcommerceAPI.Business.DependencyInjection;
using EcommerceAPI.DataAccess.DependencyInjection;
using EcommerceAPI.Middleware;
using EcommerceAPI.DataAccess.Context;
using EcommerceAPI.DataAccess.Identity;
using EcommerceAPI.Core.Abstractions;
using EcommerceAPI.Services.Email;
using EcommerceAPI.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Keep JWT claim types as-is (don’t remap “sub” -> NameIdentifier)
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// API layer
builder.Services.AddApiServices();

// Business & DataAccess
builder.Services.AddBusinessServices();
builder.Services.AddDataAccessServices(builder.Configuration);

// Identity
builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ECommerceDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

// JWT
var key = builder.Configuration["Jwt:Key"] ?? "dev-key-change-me";
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        o.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JWT");
                logger.LogWarning(ctx.Exception, "JWT authentication failed");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JWT");
                if (string.IsNullOrEmpty(ctx.Request.Headers["Authorization"]))
                    logger.LogWarning("JWT challenge: Authorization header missing");
                else
                    logger.LogWarning("JWT challenge: Authorization header present but invalid. Header={Auth}", ctx.Request.Headers["Authorization"].ToString());
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Infra
builder.Services.AddSingleton<IEmailSender, DebugEmailSender>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Swagger with Bearer scheme
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EcommerceAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference
            { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
