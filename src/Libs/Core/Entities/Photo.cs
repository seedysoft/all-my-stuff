namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)},nq}}")]
public sealed class Photo
{
    public long PhotoId { get; set; }

    public required string FilePath { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Models.Location? Location => Latitude.HasValue && Longitude.HasValue ? new(Latitude.Value, Longitude.Value) : null;

    public DateTimeOffset? UpdatedAtDateTimeOffset { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

    private string GetDebuggerDisplay() => throw new NotImplementedException();
}
