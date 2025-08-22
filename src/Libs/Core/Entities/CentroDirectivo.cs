namespace Seedysoft.Libs.Core.Entities;

public class CentroDirectivo
{
    public int CentroDirectivoId { get; set; }

    public required string CentroDirectivoDenominacion { get; set; }

    public int MinisterioId { get; set; }

    public required Ministerio Ministerio {  get; set; }

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];
}
