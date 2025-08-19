namespace Seedysoft.Libs.Core.Entities;

public record TiposPuesto(string Value) : MasterFilesBase(Value)
{
#pragma warning disable format
    public static readonly TiposPuesto EventualOrFuncionario = new("E");
    public static readonly TiposPuesto NoSingularizado       = new("N");
    public static readonly TiposPuesto Singularizado         = new("S");
#pragma warning restore format

    public static implicit operator TiposPuesto?(string s)
    {
        return s switch
        {
            "E" => EventualOrFuncionario,
            "N" => NoSingularizado,
            "S" => Singularizado,
            null => null,
            "" => null,
            _ => throw new NotImplementedException()
        };
    }
}
