namespace Seedysoft.CoreLib.Entities;

public abstract class WebDataBase
{
    public long SubscriptionId { get; set; }

    public string WebUrl { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string? CurrentWebContent { get; set; }

    public DateTimeOffset? SeenAtDateTimeOffset { get; set; }

    public DateTimeOffset? UpdatedAtDateTimeOffset { get; set; }

    public string? IgnoreChangeWhen { get; set; }

    public string CssSelector { get; set; } = default!;

    public long TakeAboveBelowLines { get; set; }
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed partial class WebData : WebDataBase
{
    public WebData(string webUrl, string description)
    {
        WebUrl = webUrl;
        Description = description;
    }

    private string GetDebuggerDisplay() => $"{WebUrl}";
}

public sealed partial class WebData
{
    public string Hyperlink => $"<a href=\"{WebUrl}\">{(string.IsNullOrWhiteSpace(Description) ? WebUrl : Description)}</a>";

    public string? DataToSend { get; set; }
}

public sealed class WebDataView : WebDataBase
{
    public long? SeenAtDateTimeUnix { get; set; }
    public long? UpdatedAtDateTimeUnix { get; set; }
}
