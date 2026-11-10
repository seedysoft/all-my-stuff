namespace Seedysoft.Photo.Lib.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Photo
{
    public long PhotoId { get; set; }

    public required string FilePath { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Libs.Core.Models.Location? Location => Latitude.HasValue && Longitude.HasValue ? new(Latitude.Value, Longitude.Value) : null;

    public int Year { get; set; }

    //public DateTimeOffset? UpdatedAtDateTimeOffset { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

    private string GetDebuggerDisplay() => throw new NotImplementedException();
}
