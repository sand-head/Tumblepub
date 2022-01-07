using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using Tumblepub.Database.Extensions;
using Tumblepub.GraphQL;
using Tumblepub.Infrastructure;
using Tumblepub.Services;
using Tumblepub.Themes;

Helpers.Register();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Host.UseSerilog();

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
    .AddScoped<IUserService, UserService>()
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

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/@{name}", (string name) =>
{
    var data = new
    {
        Title = name,
        Avatar = $"/api/assets/avatar/{name}",
        Posts = new List<object>()
    };

    // todo: add ThemeService to allow for custom themes
    var page = DefaultTheme.Template.Value(data);
    return Results.Content(page, "text/html");
});

// todo: show blog in "single user" mode
app.MapGet("/", () => Results.NotFound());

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.MapControllers();

app.Run();
