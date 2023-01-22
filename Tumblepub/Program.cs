using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json;
using Tumblepub;
using Tumblepub.ActivityPub.Extensions;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Configuration;
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
    .AddActivityPub()
    .AddMemoryCache()
    .AddScoped<IRenderService, RenderService>();

// add AutoMapper configuration
builder.Services.AddAutoMapper(typeof(GraphQLProfile));

// add GraphQL support using HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddUnionType<PostContent>()
    .AddType<PostContent.External>()
    .AddType<PostContent.Internal>()
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
        var userRepository = scope.ServiceProvider.GetService<IRepository<User, Guid>>()!;
        var blogRepository = scope.ServiceProvider.GetService<IRepository<Blog, Guid>>()!;

        // if "single user" user doesn't exist, create new user and blog
        if (await userRepository.GetByEmailAsync(singleUserConfig.Email) == null)
        {
            var user = new User(singleUserConfig.Email, singleUserConfig.Password);
            await userRepository.CreateAsync(user);
            await userRepository.SaveChangesAsync();
            
            var blog = new Blog(user.Id, singleUserConfig.Username);
            await blogRepository.CreateAsync(blog);
            await blogRepository.SaveChangesAsync();
        }
    }
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseActivityPub();

app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path.Value;
    var singleUserConfig = context.RequestServices.GetService<IOptions<SingleUserConfiguration>>()?.Value;
    
    // if "single user" is enabled, redirect all requests to "/" to the single user's blog
    if (singleUserConfig != null && !(requestPath.StartsWith("/@") || requestPath.StartsWith("/api")))
    {
        var newPath = $"/@{singleUserConfig.Username}{requestPath}";
        if (newPath.EndsWith("/"))
        {
            newPath = newPath[..^1];
        }
        
        context.Request.Path = newPath;
    }
    
    await next();
});

app.UseRouting();

app.MapGet("/@{name}", async (string name, IRenderService renderService) =>
{
    return await renderService.RenderBlogAsync(name);
});

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.MapControllers();

app.Run();
