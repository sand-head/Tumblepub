namespace Tumblepub.Themes;

public record RenderedPost(string Content, DateTime CreatedAt, string Author);

public record ThemeVariables(
    string Title,
    string? Description,
    string Avatar,
    IEnumerable<RenderedPost> Posts);
