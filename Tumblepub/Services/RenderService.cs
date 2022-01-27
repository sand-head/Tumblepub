using Tumblepub.Themes;

namespace Tumblepub.Services;

public interface IRenderService
{
    Task<IResult> RenderBlogAsync(string name, CancellationToken token = default);
}

public class RenderService : IRenderService
{
    private readonly IBlogService _blogService;

    public RenderService(IBlogService blogService)
    {
        _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
    }

    public async Task<IResult> RenderBlogAsync(string name, CancellationToken token = default)
    {
        var blog = await _blogService.GetByNameAsync(name, null, token);
        if (blog == null)
        {
            return Results.NotFound();
        }

        var data = new
        {
            Title = blog.Title ?? blog.BlogName,
            Description = blog.Description,
            Avatar = $"/api/assets/avatar/{blog.BlogName}",
            Posts = new List<object>()
        };

        // todo: add a ThemeService to allow for custom themes
        var page = DefaultTheme.Template.Value(data);
        return Results.Content(page, "text/html");
    }
}
