namespace Seedysoft.Photo.Lib.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Tag
{
    public long TagId { get; set; }

    public required string Text { get; set; }

    public ICollection<Photo> Photos { get; set; } = [];

    private string GetDebuggerDisplay() => throw new NotImplementedException();
}
