using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CarePoint.API.Configuration;
using CarePoint.API.Data;
using CarePoint.API.Interfaces;
using CarePoint.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DB context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register application service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add authentication with JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Null check - importing to prevent runtime errors
if(jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT settings are not configured properly in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
    };
});

// Add authorization
builder.Services.AddAuthorization();

// Add CORS
var corsOrigin = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

// Null check for CORS origins
if(corsOrigin == null || corsOrigin.Length == 0)
{
    corsOrigin = new[] { "http://localhost:5173", "http://localhost:3000" };
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigin)
            .AllowAnyMethod();
            .AllowAnyHeader();
            .AllowCredentials();
    });
});

// Add controllers
builder.Services.AddController();

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo)
    {
        Title = "CarePoint API",
        Version = "v1",
        Description = "Healthcare Management System API"
    };

    // Add JWT authentication to swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your taken",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware pipeline
// Order matters. 
// Configure the HTTP request pipeline
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggeUI();
}

app.HttpRedirection();

// CORS must come before Authentication/Authorization
app.UseCors("AllowFrontend");

// Authentication must come before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




