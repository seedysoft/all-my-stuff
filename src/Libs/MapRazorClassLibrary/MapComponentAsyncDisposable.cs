namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    #region IAsyncDisposable Impl

    public async ValueTask DisposeAsync()
    {
        try
        {
            ObjRef?.Dispose();
            if (MapModule is not null)
                await MapModule.DisposeAsync();

            GC.SuppressFinalize(this);
        }
        catch (Microsoft.JSInterop.JSDisconnectedException) { }
    }

    #endregion
}
