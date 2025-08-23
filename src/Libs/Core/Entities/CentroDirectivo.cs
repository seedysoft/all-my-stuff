namespace Seedysoft.Libs.Core.Entities;

public class CentroDirectivo
{
    public int CentroDirectivoId { get; set; }

    public required string CentroDirectivoDenominacion { get; set; }

    public int MinisterioId { get; set; }

    public Ministerio Ministerio { get; set; } = default!;

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];
}
