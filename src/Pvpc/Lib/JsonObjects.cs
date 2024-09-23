// TODO:                 Remove warning disable and use attributes

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable IDE1006 // Naming Styles
public sealed class Rootobject
{
    public Data? data { get; set; }
    public Included[]? included { get; set; }
}

public sealed class Data
{
    public string? type { get; set; }
    public string? id { get; set; }
    public Attributes? attributes { get; set; }
    public Meta? meta { get; set; }
}

public sealed class Attributes
{
    public string? title { get; set; }
    public DateTime lastupdate { get; set; }
    public object? description { get; set; }
}

public sealed class Meta
{
    public CacheControl? cachecontrol { get; set; }
}

public sealed class CacheControl
{
    public string? cache { get; set; }
    public DateTime? expireAt { get; set; }
}

public sealed class Included
{
    public string? type { get; set; }
    public string? id { get; set; }
    public object? groupId { get; set; }
    public Attributes1? attributes { get; set; }
}

public sealed class Attributes1
{
    public string? title { get; set; }
    public object? description { get; set; }
    public string? color { get; set; }
    public object? type { get; set; }
    public string? magnitude { get; set; }
    public bool? composite { get; set; }
    public DateTime? lastupdate { get; set; }
    public Value[]? values { get; set; }
}

public sealed class Value
{
    public float? value { get; set; }
    public float? percentage { get; set; }
    public DateTimeOffset? datetime { get; set; }
}
