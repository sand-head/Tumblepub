using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using Tumblepub.Database.Extensions;
using Tumblepub.GraphQL;
using Tumblepub.Infrastructure;
using Tumblepub.Models;
using Tumblepub.Services;
using Tumblepub.Themes;

Helpers.Register();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
ConfigurationManager? config = builder.Configuration;

builder.Host.UseSerilog();

builder.Services
    .Configure<SingleUserConfiguration>(config.GetSection("SingleUser"));

// add domain services
builder.Services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IBlogService, BlogService>();

// configure event sourcing
builder.AddEventSourcing(config.GetConnectionString("Database"));

// add GraphQL support using HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

builder.Services
    .AddSingleton<JwtTokenConfig>(new JwtTokenConfig(
        config["LocalDomain"],
        config["JwtSecret"],
        AccessTokenExpiration: (int)TimeSpan.FromHours(2).TotalMinutes,
        RefreshTokenExpiration: (int)TimeSpan.FromDays(90).TotalMinutes))
    .AddSingleton<JwtAuthenticationManager>()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        x.SaveToken = true;
        x.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = config["LocalDomain"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtSecret"])),
            //ValidAudience = jwtTokenConfig.Audience,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddControllers();

WebApplication? app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/@{name}", async (string name, IBlogService blogService) =>
{
    return await blogService.RenderAsync(name);
});

// todo: show blog in "single user" mode
app.MapGet("/", async (IOptions<SingleUserConfiguration> singleUserConfig, IBlogService blogService) =>
{
    return singleUserConfig.Value is not null ? await blogService.RenderAsync(singleUserConfig.Value.Username) : Results.NotFound();
});

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.MapControllers();

app.Run();
