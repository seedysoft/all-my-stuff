using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib;

public partial class ScriptsLoader : SeedysoftComponentBase
{
    /// <summary>
    /// The default content type for scripts.
    /// </summary>
    private const string ScriptType = "text/javascript";

    /// <summary>
    /// A reference to this component instance for use in JavaScript calls.
    /// </summary>
    private DotNetObjectReference<ScriptsLoader> objRef = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftScriptLoader}.initialize", Id, Async, Defer, ScriptId, Source, ScriptType, objRef);

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        objRef ??= DotNetObjectReference.Create(this);

        await base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Source))
            throw new ArgumentNullException(nameof(Source));

        base.OnParametersSet();
    }

    /// <summary>
    /// Handles a script error event from JavaScript.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    [JSInvokable]
    public void OnErrorJS(string errorMessage)
    {
        if (OnError.HasDelegate)
            _ = OnError.InvokeAsync(errorMessage);
    }

    /// <summary>
    /// Handles a script load event from JavaScript.
    /// </summary>
    [JSInvokable]
    public void OnLoadJS()
    {
        if (OnLoad.HasDelegate)
            _ = OnLoad.InvokeAsync();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the script should be loaded asynchronously.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="false" />.
    /// </remarks>
    [Parameter] public bool Async { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the script is meant to be executed 
    /// after the document has been parsed, but before firing DOMContentLoaded event..
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="false" />.
    /// </remarks>
    [Parameter] public bool Defer { get; set; }

    /// <summary>
    /// An event that is fired when a script loading error occurs.
    /// </summary>
    [Parameter] public EventCallback<string> OnError { get; set; }

    /// <summary>
    /// An event that is fired when a script has been successfully loaded.
    /// </summary>
    [Parameter] public EventCallback OnLoad { get; set; }

    /// <summary>
    /// Gets or sets the ID of the script element.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public string? ScriptId { get; set; }

    /// <summary>
    /// Gets or sets the URI of the external script to load.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [EditorRequired]
    [Parameter] public string? Source { get; set; } = default!;
}
