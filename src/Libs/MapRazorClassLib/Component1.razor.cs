using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLib;

public partial class Component1 : ComponentBase
{
    [EditorRequired]
    [Parameter] public required string Id { get; set; }

    public ElementReference Element { get; set; }

    [Inject] protected IJSRuntime JsRuntime { get; set; } = default!;

    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/Seedysoft.Libs.MapRazorClassLib/map.js");

        await _module.InvokeVoidAsync($"createMap", Id, null);
    }
}
