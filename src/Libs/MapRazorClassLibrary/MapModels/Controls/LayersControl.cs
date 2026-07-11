namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

public sealed class LayersControl : IControl
{
    public string Id { get; }

    internal LayersControl(string id) => Id = id;
}
