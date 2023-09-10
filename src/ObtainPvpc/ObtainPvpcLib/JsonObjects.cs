#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable IDE1006 // Naming Styles
public class Rootobject
{
    public Data? data { get; set; }
    public Included[]? included { get; set; }
}

public class Data
{
    public string? type { get; set; }
    public string? id { get; set; }
    public Attributes? attributes { get; set; }
    public Meta? meta { get; set; }
}

public class Attributes
{
    public string? title { get; set; }
    public DateTime lastupdate { get; set; }
    public object? description { get; set; }
}

public class Meta
{
    public CacheControl? cachecontrol { get; set; }
}

public class CacheControl
{
    public string? cache { get; set; }
    public DateTime? expireAt { get; set; }
}

public class Included
{
    public string? type { get; set; }
    public string? id { get; set; }
    public object? groupId { get; set; }
    public Attributes1? attributes { get; set; }
}

public class Attributes1
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

public class Value
{
    public float? value { get; set; }
    public float? percentage { get; set; }
    public DateTimeOffset? datetime { get; set; }
}
#pragma warning restore CA1050 // Declare types in namespaces
#pragma warning restore IDE1006 // Naming Styles
