using Microsoft.AspNetCore.Mvc;
using Tumblepub.Database.Projections;
using Tumblepub.Themes;

namespace Tumblepub.Services;

public interface IBlogService
{
    Task<Blog> CreateAsync(Guid userId, string name);
    Task<IResult> RenderAsync(string name);
}

public class BlogService : IBlogService
{
    public Task<Blog> CreateAsync(Guid userId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> RenderAsync(string name)
    {
        // todo: get blog from projection
        var data = new
        {
            Title = name,
            Avatar = $"/api/assets/avatar/{name}",
            Posts = new List<object>()
        };

        // todo: add ThemeService to allow for custom themes
        var page = DefaultTheme.Template.Value(data);
        return Results.Content(page, "text/html");
    }
}
