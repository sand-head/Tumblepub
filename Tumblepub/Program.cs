using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using System.Text;
using Tumblepub.EventSourcing.Extensions;
using Tumblepub.GraphQL;
using Tumblepub.Infrastructure;
using Tumblepub.Themes;

Helpers.Register();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// configure event sourcing
builder.AddEventSourcing(config.GetConnectionString("Database"));

// add GraphQL support using HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

builder.Services
    .AddSingleton<JwtTokenConfig>(new JwtTokenConfig(config["LocalDomain"], config["JwtSecret"], 60 * 2, 60 * 24))
    .AddSingleton<JwtAuthenticationManager>()
    .AddAuthorization()
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/@{name}", (string name) =>
{
    var data = new
    {
        Title = name,
        Avatar = "https://pbs.twimg.com/profile_images/1444764955505532929/1l5wrYIs_400x400.jpg",
        Posts = new List<object>()
    };

    // todo: add ThemeService to allow for custom themes
    var page = DefaultTheme.Template.Value(data);
    return Results.Content(page, "text/html");
});

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.Run();
