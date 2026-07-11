namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

public sealed class ZoomControl : IControl
{
    public string Id { get; }

    internal ZoomControl(string id) => Id = id;
}
