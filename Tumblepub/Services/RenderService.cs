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
    private readonly IBlogRepository _blogRepository;

    public RenderService(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
    }

    public async Task<IResult> RenderBlogAsync(string name, CancellationToken token = default)
    {
        var blog = await _blogRepository.GetByNameAsync(name, null, token);
        if (blog == null)
        {
            return Results.NotFound();
        }

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
