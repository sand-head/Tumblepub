using Tumblepub.Application.Models;

namespace Tumblepub.Themes;

public record ThemeVariables(
    string Title,
    string? Description,
    string Avatar,
    List<Post> Posts);
