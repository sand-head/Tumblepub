﻿using Marten.Schema;

namespace Tumblepub.Database.Models;

public enum ObjectType
{
    Blog,
    Post
}

public class BlogActivity
{
    public Guid Id { get; set; }
    [ForeignKey(typeof(Blog))]
    public Guid BlogId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }

    public ObjectType ObjectType { get; set; }
    public Guid ObjectId { get; set; }

    public ObjectType TargetType { get; set; }
    public Guid TargetId { get; set; }

    public ObjectType OriginType { get; set; }
    public Guid OriginId { get; set; }
}