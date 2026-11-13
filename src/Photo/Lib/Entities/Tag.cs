namespace Seedysoft.Photo.Lib.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Tag
{
    internal static readonly Tag NoGeoLocation = new()
    {
        Text = "Sin Localización",
        TagGuid = Guid.Parse("8F500DAF-7BFC-4BC0-82E7-975E5D6C78FA"),
    };

    public long TagId { get; set; }

    public required string Text { get; set; }

    public required Guid TagGuid { get; set; }

    public ICollection<Photo> Photos { get; set; } = [];

    private string GetDebuggerDisplay() => Text;
}
