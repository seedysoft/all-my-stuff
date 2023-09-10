namespace Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class WebData
{
    protected WebData() { }
    public WebData(string webUrl, string description)
    {
        WebUrl = webUrl;
        Description = description;
    }

    public long SubscriptionId { get; set; }

    public string WebUrl { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string? CurrentWebContent { get; set; }

    public DateTimeOffset? SeenAtDateTimeOffset { get; set; }

    public DateTimeOffset? UpdatedAtDateTimeOffset { get; set; }

    public string? IgnoreChangeWhen { get; set; }

    public string CssSelector { get; set; } = default!;

    public long TakeAboveBelowLines { get; set; }

    protected string GetDebuggerDisplay() => $"{WebUrl}";
}

public partial class WebData
{
    public string Hyperlink => $"<a href=\"{WebUrl}\">{(string.IsNullOrWhiteSpace(Description) ? WebUrl : Description)}</a>";

    public string? DataToSend { get; set; }
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class WebDataView : WebData
{
    public long? SeenAtDateTimeUnix { get; set; }
    public long? UpdatedAtDateTimeUnix { get; set; }
}
