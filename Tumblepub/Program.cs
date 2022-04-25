using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json;
using Tumblepub;
using Tumblepub.ActivityPub.Extensions;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Tumblepub.Configuration;
using Tumblepub.Extensions;
using Tumblepub.Infrastructure;
using Tumblepub.Infrastructure.Extensions;
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

builder.Services
    .Configure<SingleUserConfiguration>(config.GetSection("SingleUser"));

// add domain services
builder.Services
    .AddApplication()
    .AddInfrastructure(config.GetConnectionString("Database"), builder.Environment.IsDevelopment())
    .AddScoped<IRenderService, RenderService>();

// add GraphQL support using HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddUnionType<PostContent>()
    .AddType<PostContent.External>()
    .AddType<PostContent.Markdown>()
    .AddTypeExtension<UserTypeExtensions>()
    .AddTypeExtension<BlogTypeExtensions>()
    .BindRuntimeType<JsonDocument, AnyType>();

builder.Services
    .AddSingleton(new JwtTokenConfig(
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

builder.Services.AddActivityPub<ActivityPubService>(options =>
{
    options.MapActorProfileUrl = (actor) =>
    {
        return $"/@{actor.Name}";
    };
});

builder.Services.AddControllers();

var app = builder.Build();

// create single user on "first run"
// (I don't actually care if it's the first run)
// (so if people want to create additional accounts that's whatever)
using (var scope = app.Services.CreateScope())
{
    var singleUserConfig = scope.ServiceProvider.GetService<IOptions<SingleUserConfiguration>>()?.Value;
    if (singleUserConfig != null)
    {
        var userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
        var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

        // if "single user" user doesn't exist, create new user and blog
        if (await userRepository.GetByEmailAsync(singleUserConfig.Email) == null)
        {
            var user = await userRepository.CreateAsync(singleUserConfig.Email, singleUserConfig.Password);
            await blogRepository.CreateAsync(user.Id, singleUserConfig.Username);
        }
    }
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseActivityPub();

app.MapGet("/@{name}", async (string name, IRenderService renderService) =>
{
    return await renderService.RenderBlogAsync(name);
});

// show blog in "single user" mode
app.MapGet("/", async (IOptions<SingleUserConfiguration> singleUserConfig, IRenderService renderService) =>
{
    return singleUserConfig.Value is not null
        ? await renderService.RenderBlogAsync(singleUserConfig.Value.Username)
        : Results.NotFound();
});

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.MapControllers();

app.Run();
