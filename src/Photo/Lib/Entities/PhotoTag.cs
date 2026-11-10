namespace Seedysoft.Photo.Lib.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class PhotoTag
{
    public long PhotoId { get; set; }

    public long TagId {  get; set; }

    private string GetDebuggerDisplay() => $"{PhotoId} etiquetada con {TagId}";
}
