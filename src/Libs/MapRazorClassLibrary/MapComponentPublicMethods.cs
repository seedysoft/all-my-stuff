using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    #region IAsyncDisposable Impl
    public async ValueTask DisposeAsync()
    {
        await DeleteMapAsync();
        ObjRef?.Dispose();
        await MapModule.DisposeAsync();
        GC.SuppressFinalize(this);
    }
    #endregion

    public IJSObjectReference MapModule { get => field!; private set; }
}
