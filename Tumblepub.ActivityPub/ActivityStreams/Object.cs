using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record Object(string Type) : ActivityStreamsValue(Type)
{
    /// <summary>
    /// Provides the globally unique identifier for an object.
    /// </summary>
    /// <remarks>
    /// If relative, the scheme, domain, and port will be added in from the ActivityPub request.
    /// </remarks>
    public Uri Id { get; set; } = default!;

    /// <summary>
    /// Identifies a resource attached or related to an object that potentially requires special handling. The intent is to provide a model that is at least semantically similar to attachments in email.
    /// </summary>
    public List<ActivityStreamsValue>? Attachment { get; set; }
    /// <summary>
    /// Identifies one or more entities to which this object is attributed. The attributed entities might not be Actors. For instance, an object might be attributed to the completion of another activity.
    /// </summary>
    public List<ActivityStreamsValue>? AttributedTo { get; set; }
    /// <summary>
    /// Identifies one or more entities that represent the total population of entities for which the object can considered to be relevant.
    /// </summary>
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Audience { get; set; }
    /// <summary>
    /// The content or textual representation of the Object encoded as a JSON string. By default, the value of content is HTML. <see cref="MediaType"/> can be used to indicate a different content type.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Identifies an entity considered to be part of the public primary audience of an Object.
    /// </summary>
    public List<ActivityStreamsValue>? To { get; set; }
    /// <summary>
    /// Identifies an Object that is part of the private primary audience of this Object.
    /// </summary>
    public List<ActivityStreamsValue>? Bto { get; set; }
    /// <summary>
    /// Identifies an Object that is part of the public secondary audience of this Object.
    /// </summary>
    public List<ActivityStreamsValue>? Cc { get; set; }
    /// <summary>
    /// Identifies one or more Objects that are part of the private secondary audience of this Object.
    /// </summary>
    public List<ActivityStreamsValue>? Bcc { get; set; }

    /// <summary>
    /// Identifies the MIME media type of the value of the content property. If not specified, the content property is assumed to contain text/html content.
    /// </summary>
    public string? MediaType { get; set; }
    /// <summary>
    /// When the object describes a time-bound resource, such as an audio or video, a meeting, etc, the duration property indicates the object's approximate duration.
    /// </summary>
    /// <remarks>
    /// The value MUST be expressed as an xsd:duration as defined by [xmlschema11-2](https://www.w3.org/TR/activitystreams-vocabulary/#bib-xmlschema11-2), section 3.3.6 (e.g. a period of 5 seconds is represented as "PT5S").
    /// </remarks>
    public string? Duration { get; set; }
}
