namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

public record Evented
{
    [J("options")] public virtual EventedOptions? Options { get; }

    public Evented(EventedOptions? eventedOptions = default) => Options = eventedOptions;
}

public record EventedOptions
{
    // Nothing here
}
