using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib;

public abstract class SeedysoftComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
    #region Parameters

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = default!;

    [Parameter] public string? Class { get; set; }

    [EditorRequired]
    [Parameter] public required string Id { get; set; }

    [Parameter] public string? Style { get; set; }

    #endregion

    private bool isAsyncDisposed;
    private bool isDisposed;

    [Inject] protected IConfiguration Configuration { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        IsRenderComplete = true;

        await base.OnAfterRenderAsync(firstRender);
    }

    public static string BuildClassNames(params (string? cssClass, bool when)[] cssClassList)
        => BuildClassNames(null, cssClassList);
    public static string BuildClassNames(string? userDefinedCssClass, params (string? cssClass, bool when)[] cssClassList)
    {
        var list = cssClassList
            .Where(static x => x.when && !string.IsNullOrWhiteSpace(x.cssClass))
            .Select(static x => x.cssClass)
            .ToHashSet();

        if (!string.IsNullOrWhiteSpace(userDefinedCssClass))
            _ = list.Add(userDefinedCssClass.Trim());

        return list.Count == 0 ? string.Empty : string.Join(" ", list);
    }
    public static string BuildStyleNames(string? userDefinedCssStyle, params (string? cssStyle, bool when)[] cssStyleList)
    {
        var list = cssStyleList
            .Where(static x => x.when && !string.IsNullOrWhiteSpace(x.cssStyle))
            .Select(static x => x.cssStyle)
            .ToHashSet();

        if (!string.IsNullOrWhiteSpace(userDefinedCssStyle))
            _ = list.Add(userDefinedCssStyle.Trim());

        return list.Count == 0 ? string.Empty : string.Join(';', list);
    }

    protected virtual string? ClassNames => Class;

    public ElementReference Element { get; set; }

    protected bool IsRenderComplete { get; private set; }

    protected virtual string? StyleNames => Style;

    ~SeedysoftComponentBase()
    {
        Dispose(false);
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true).ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                // cleanup
            }

            isDisposed = true;
        }
    }
    protected virtual ValueTask DisposeAsyncCore(bool disposing)
    {
        if (!isAsyncDisposed)
        {
            if (disposing)
            {
                // cleanup
            }

            isAsyncDisposed = true;
        }

        return ValueTask.CompletedTask;
    }
}
