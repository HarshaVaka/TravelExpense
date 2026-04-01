using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Data;
using UserService.Api.Models;
using UserService.Api.Repositories;
using UserService.Api.Services;
using UserService.Api.Dtos;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserService.Api.Middleware;
using Serilog;
using Serilog.Events;
using UserService.Api.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// configure Serilog early so startup logs are captured
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add controllers (MVC) so FluentValidation can participate in model validation
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register RabbitMqConfig
builder.Services.Configure<RabbitMqConfig>(
    builder.Configuration.GetSection("RabbitMq"));

// Register application services and repositories (skeletons)
builder.Services.AddScoped<IUserRepository,UserRepository>();
// register password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService,UserServiceImpl>();

// FluentValidation - register validators from this assembly and enable auto validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// Configure a consistent 400 response shape for model validation failures
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kv => kv.Value != null && kv.Value.Errors.Count > 0)
            .SelectMany(kv => kv.Value!.Errors.Select(e => new { field = kv.Key, error = e.ErrorMessage }))
            .ToArray();

        var result = new BadRequestObjectResult(new { message = "Validation failed", errors });
        return result;
    };
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value;

    var factory = new ConnectionFactory()
    {
        HostName = settings.HostName,
        UserName = settings.UserName,
        Password = settings.Password
    };

    return factory.CreateConnection();
});

// refresh token repository and jwt service
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();


// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// global exception handling middleware should be early in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Map controllers
app.MapControllers();


app.Run();


