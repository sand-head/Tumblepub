using Ganss.Xss;
using Markdig;
using Mediator;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Post.Queries;
using Tumblepub.Themes;

namespace Tumblepub.Services;

public interface IRenderService
{
    Task<IResult> RenderBlogAsync(string name, CancellationToken token = default);
}

public class RenderService : IRenderService
{
    private static readonly HtmlSanitizer Sanitizer;
    private readonly IMediator _mediator;
    
    static RenderService()
    {
        Sanitizer = new();
        Sanitizer.AllowedCssProperties.Add("-webkit-text-stroke");
        Sanitizer.AllowedCssProperties.Add("-webkit-text-stroke-color");
        Sanitizer.AllowedCssProperties.Add("-webkit-text-stroke-width");
    }

    public RenderService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IResult> RenderBlogAsync(string name, CancellationToken token = default)
    {
        var blogQuery = new GetBlogByNameQuery(name);
        var blog = await _mediator.Send(blogQuery, token);
        if (blog == null)
        {
            return Results.NotFound();
        }
        
        var postsQuery = new GetPostsByBlogIdQuery(blog.Id, 0, 25);
        var posts = await _mediator.Send(postsQuery, token);

        var renderedPosts = await Task.WhenAll(posts.Select(p => RenderPostAsync(p, token)));
        var data = new ThemeVariables(
            Title: blog.Title ?? blog.Name,
            Description: blog.Description,
            Avatar: $"/api/assets/avatar/{blog.Name}",
            Posts: renderedPosts);

        // todo: add a ThemeService to allow for custom themes
        var page = DefaultTheme.Template.Value(data);
        return Results.Content(page, "text/html");
    }

    private Task<RenderedPost> RenderPostAsync(Post post, CancellationToken token = default)
    {
        // todo: resolve all external posts (using additional service)
        if (post.Content is not PostContent.Internal markdown)
        {
            throw new Exception("Post content is not Markdown");
        }
        
        // convert Markdown to HTML
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        var html = Markdown.ToHtml(markdown.Content, pipeline);
        
        // sanitize the resulting HTML
        var sanitizedHtml = Sanitizer.Sanitize(html);
        
        return Task.FromResult(new RenderedPost(sanitizedHtml, post.CreatedAt.DateTime, post.BlogId.ToString()));
    }
}
