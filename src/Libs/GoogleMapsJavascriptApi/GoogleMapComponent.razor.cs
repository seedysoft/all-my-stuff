using GoogleMapsLibrary.Extensions;
using GoogleMapsLibrary.Interfaces;
using GoogleMapsLibrary.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace GoogleMapsJavascriptApi;

public partial class GoogleMapComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject] public IJSRuntime JsRuntime { get; protected set; } = default!;
    [Inject] public IServiceProvider ServiceProvider { get; protected set; } = default!;

    private IBlazorGoogleMapsKeyService? _keyService;
    private ElementReference Element { get; set; }
    private string StyleStr => $"height: {Height};";

    [Parameter] public string ApiKey { get; set; } = string.Empty;

    [Parameter] public string? CssClass { get; set; }

    private string _height = "500px";
    /// <summary>
    /// Default height 500px.
    /// Used as style atribute "height: {Height}".
    /// </summary>
    [Parameter]
    public string Height
    {
        get => _height;
        set => _height = value ?? "500px";
    }

    [Parameter] public string Id { get; set; } = default!;

    [Parameter] public EventCallback OnAfterInit { get; set; }

    [Parameter] public MapOptions? Options { get; set; }

    public Map InteropObject { get; private set; } = default!;

    protected override void OnInitialized()
    {
        // get the service from the provider instead of with [Inject] in case no service was registered. e.g. when the user loads the api with a script tag.
        _keyService = ServiceProvider.GetService<IBlazorGoogleMapsKeyService>();
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //var tasks = new List<Task<IJSObjectReference>>();
            //tasks.Add(JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorGoogleMaps/js/objectManager.js").AsTask());
            //if(!string.IsNullOrWhiteSpace(ApiKey))
            //    tasks.Add(JSRuntime.InvokeAsync<IJSObjectReference>("import", $"https://maps.googleapis.com/maps/api/js?key={ApiKey}&v=3").AsTask());

            //moduleImports.AddRange(await Task.WhenAll(tasks.ToArray()));
        }

        await InitAsync(Element, Options);

        //Debug.WriteLine("Init finished");

        await OnAfterInit.InvokeAsync();
    }

    public async Task InitAsync(ElementReference element, MapOptions? options = null)
    {
        if (options?.ApiLoadOptions == null && _keyService != null && !_keyService.IsApiInitialized)
        {
            _keyService.IsApiInitialized = true;
            options ??= new MapOptions();
            options.ApiLoadOptions = await _keyService.GetApiOptions();
        }

        InteropObject = await Map.CreateAsync(JsRuntime, element, options);
    }

    protected override bool ShouldRender() => false;

    #region Dispose

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~GoogleMapComponent()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (InteropObject is not null)
        {
            try
            {
                await InteropObject.DisposeAsync();
                InteropObject = null!;
            }
            catch (Exception ex)
            {
                bool isPossibleRefreshError =
                    ex.HasInnerExceptionsOfType<TaskCanceledException>() ||
                    ex.HasInnerExceptionsOfType<ObjectDisposedException>() ||
                    ex.HasInnerExceptionsOfType<JSDisconnectedException>();

                if (!isPossibleRefreshError)
                    throw;
            }

            GC.SuppressFinalize(this);
        }
    }

    #endregion
}
