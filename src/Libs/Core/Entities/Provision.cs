namespace Seedysoft.Libs.Core.Entities;

public record FormasProvision(string Value) : MasterFilesBase(Value)
{
    public const string ConcursoDeMeritos = "C";

    public const string EventualOrFuncionario = "I";

    public const string LibreDesignacion = "L";
}
