namespace Tumblepub.ActivityPub.ActivityStreams;

public record Collection() : Object("Collection")
{
    public static new readonly string[] Types = new[] { "Collection" };
}
