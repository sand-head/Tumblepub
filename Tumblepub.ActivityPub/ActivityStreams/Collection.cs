namespace Tumblepub.ActivityPub.ActivityStreams;

public record Collection() : ActivityStreamsObject("Collection")
{
    public static new readonly string[] Types = new[] { "Collection" };
}
