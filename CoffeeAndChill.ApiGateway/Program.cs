using System.Text;
using CoffeeAndChill.ApiGateway.Models;
using CoffeeAndChill.ApiGateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<UserTableService>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

// JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// Swagger (with a Bearer "Authorize" button)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the token from /api/auth/login"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddHttpClient("Functions", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Functions:BaseUrl"]!);
});

var app = builder.Build();

// Seed an Admin user if one doesn't exist yet
var seedEmail = builder.Configuration["Seed:AdminEmail"];
var seedPassword = builder.Configuration["Seed:AdminPassword"];
if (!string.IsNullOrWhiteSpace(seedEmail) && !string.IsNullOrWhiteSpace(seedPassword))
{
    using var scope = app.Services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<UserTableService>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserEntity>>();

    if (await userService.GetUserByEmailAsync(seedEmail) == null)
    {
        var admin = new UserEntity
        {
            Name = "Administrator",
            Email = seedEmail.Trim().ToLowerInvariant(),
            Role = "Admin"
        };
        admin.PasswordHash = hasher.HashPassword(admin, seedPassword);
        await userService.CreateUserAsync(admin);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();   // must come BEFORE UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:8080");