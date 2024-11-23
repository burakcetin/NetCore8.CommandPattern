using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore8.CommandPattern.Infrastructure;
using NetCore8.CommandPattern.Infrastructure.Data;
using NetCore8.CommandPattern.Infrastructure.DependencyInjection;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;
using Prometheus;
using StackExchange.Redis;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
var builder = WebApplication.CreateBuilder(args);

var lokiLabels = new[]
{
    new LokiLabel { Key = "app", Value = "command-pattern" },
    new LokiLabel { Key = "environment", Value = "development" },
    new LokiLabel { Key = "host", Value = Environment.MachineName }
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.GrafanaLoki("http://loki:3100", lokiLabels)
    .CreateLogger();

builder.Host.UseSerilog();

// Redis connection
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") + ",abortConnect=false";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);

// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
    .SetApplicationName("CommandPatternApp");

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("NetCore8.CommandPattern.Infrastructure")  
    );
});

// Ayrýca base DbContext olarak da kaydedelim
builder.Services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>());

// Register CommandContext
builder.Services.AddScoped<CommandContext>(provider =>
{
    var serviceProvider = provider;
    var dbContext = provider.GetRequiredService<DbContext>();
    var logger = provider.GetRequiredService<ILogger<CommandContext>>();
    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "system"),
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim("Permission", "Users.Create"),
        new Claim("Permission", "Users.Update"),
        new Claim("Permission", "Users.Delete")
    }));

    return new CommandContext(serviceProvider, dbContext, logger, user);
});

// Add Command Pattern Services
builder.Services.AddCommandPattern(options =>
{
    options.UseRedisCache = true;
    options.RedisConnectionString = builder.Configuration.GetConnectionString("Redis");
});

// Add Controllers and API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Command Pattern API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
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

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourDefaultSecretKey"))
        };
    });

var app = builder.Build();
app.UseRouting();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add Prometheus metrics middleware
app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
 

app.Run();