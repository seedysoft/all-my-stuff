namespace Seedysoft.Carburantes.Core.JsonObjects.Minetur;

public abstract record EstacionServicioBaseRootJson
{
    public string Fecha { get; set; } = default!;

    public string Nota { get; set; } = default!;

    public string ResultadoConsulta { get; set; } = default!;
}
