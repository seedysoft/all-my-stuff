namespace Seedysoft.Carburantes.CoreLib.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record IdDescRecord
{
    public IdDescRecord(int id, string desc)
    {
        Id = id;
        Desc = desc;
    }

    public int Id { get; set; }

    public string Desc { get; set; } = default!;

    public static readonly Func<IdDescRecord, string> IdDescRecordConverter = p => p?.Desc ?? "NULL";

    private string GetDebuggerDisplay() => $"{Desc} ({Id})";
}
