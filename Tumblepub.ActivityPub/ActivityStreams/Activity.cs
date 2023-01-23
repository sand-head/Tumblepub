using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record Activity(string Type) : ActivityStreamsObject(Type)
{
    public static new readonly string[] Types = new[]
    {
        "Accept", "Add", "Announce", "Arrive", "Block", "Create",
        "Delete", "Dislike", "Flag", "Follow", "Ignore", "Invite",
        "Join", "Leave", "Like", "Listen", "Move", "Offer", "Question",
        "Reject", "Read", "Remove", "TentativeReject", "TentativeAccept",
        "Travel", "Undo", "Update", "View"
    };

    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Actor { get; init; }
    public ActivityStreamsValue? Object { get; init; }
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Target { get; init; }
}
