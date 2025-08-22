namespace Seedysoft.Libs.Core.Entities;

public class Ministerio
{
    public int MinisterioId { get; set; }

    public required string MinisterioDenominacion { get; set; }

    //public virtual ICollection<CentroDirectivo> CentrosDirectivos { get; set; } = [];

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];
}
