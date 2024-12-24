using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib;

public abstract class SeedysoftComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool isAsyncDisposed;
    private bool isDisposed;

    [Inject] protected IConfiguration Configuration { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    /// <inheritdoc />
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

    /// <inheritdoc />
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    /// <inheritdoc />
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns" />
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

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = default!;

    [Parameter]
    public string? Class { get; set; }
    protected virtual string? ClassNames => Class;

    public ElementReference Element { get; set; }

    [Parameter, EditorRequired]
    public required string Id { get; set; }

    protected bool IsRenderComplete { get; private set; }

    [Parameter]
    public string? Style { get; set; }
    protected virtual string? StyleNames => Style;

    ~SeedysoftComponentBase()
    {
        Dispose(false);
    }
}
