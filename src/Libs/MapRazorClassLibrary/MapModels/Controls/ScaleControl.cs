namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

public sealed class ScaleControl : IControl
{
    public string Id { get; }

    internal ScaleControl(string id) => Id = id;
}
