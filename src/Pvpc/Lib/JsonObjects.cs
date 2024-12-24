#pragma warning disable CA1050 // Declare types in namespaces

public sealed class Rootobject
{
    [J("data")]
    public Data? Data { get; set; }

    [J("included")]
    public Included[]? Included { get; set; }
}

public sealed class Data
{
    [J("type")]
    public string? Type { get; set; }

    [J("id")]
    public string? Id { get; set; }

    [J("attributes")]
    public Attributes? Attributes { get; set; }

    [J("meta")]
    public Meta? Meta { get; set; }
}

public sealed class Attributes
{
    [J("title")]
    public string? Title { get; set; }

    [J("lastupdate")]
    public DateTime LastUpdate { get; set; }

    [J("description")]
    public object? Description { get; set; }
}

public sealed class Meta
{
    [J("cachecontrol")]
    public CacheControl? CacheControl { get; set; }
}

public sealed class CacheControl
{
    [J("cache")]
    public string? Cache { get; set; }

    [J("expireAt")]
    public DateTime? ExpireAt { get; set; }
}

public sealed class Included
{
    [J("type")]
    public string? Type { get; set; }

    [J("id")]
    public string? Id { get; set; }

    [J("groupId")]
    public object? GroupId { get; set; }

    [J("attributes")]
    public Attributes1? Attributes { get; set; }
}

public sealed class Attributes1
{
    [J("title")]
    public string? Title { get; set; }

    [J("description")]
    public object? Description { get; set; }

    [J("color")]
    public string? Color { get; set; }

    [J("type")]
    public object? Type { get; set; }

    [J("magnitude")]
    public string? Magnitude { get; set; }

    [J("composite")]
    public bool? Composite { get; set; }

    [J("lastupdate")]
    public DateTime? LastUpdate { get; set; }

    [J("values")]
    public Value[]? Values { get; set; }
}

public sealed class Value
{
    [J("value")]
    public float? Val { get; set; }

    [J("percentage")]
    public float? Percentage { get; set; }

    [J("datetime")]
    public DateTimeOffset? Datetime { get; set; }
}
