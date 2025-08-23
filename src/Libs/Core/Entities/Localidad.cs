namespace Seedysoft.Libs.Core.Entities;

public class Localidad
{
    public required int LocalidadId { get; set; }

    public required string LocalidadDenominacion { get; set; }

    public required int PaisId { get; set; }
    public Pais Pais { get; set; } = default!;

    public required int ProvinciaId { get; set; }
    public Provincia Provincia { get; set; } = default!;

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];
}
