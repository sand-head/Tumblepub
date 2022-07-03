using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Tumblepub.Themes;

namespace Tumblepub.Services;

public interface IRenderService
{
    Task<IResult> RenderBlogAsync(string name, CancellationToken token = default);
}

public class RenderService : IRenderService
{
    private readonly IQueryHandler<GetBlogByNameQuery, Blog?> _queryHandler;

    public RenderService(IQueryHandler<GetBlogByNameQuery, Blog?> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    public async Task<IResult> RenderBlogAsync(string name, CancellationToken token = default)
    {
        var query = new GetBlogByNameQuery(name);
        var blog = await _queryHandler.Handle(query, token);
        if (blog == null)
        {
            return Results.NotFound();
        }
        
        // todo: get all posts by blog
        // todo: resolve all external posts (using additional service)

        var data = new ThemeVariables(
            Title: blog.Title ?? blog.Name,
            Description: blog.Description,
            Avatar: $"/api/assets/avatar/{blog.Name}",
            Posts: new List<Post>());

        // todo: add a ThemeService to allow for custom themes
        var page = DefaultTheme.Template.Value(data);
        return Results.Content(page, "text/html");
    }
}
