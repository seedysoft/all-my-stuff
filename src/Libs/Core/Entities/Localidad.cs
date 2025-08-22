namespace Seedysoft.Libs.Core.Entities;

public class Localidad
{
    public required string LocalidadId { get; set; }

    public required string LocalidadDenominacion { get; set; }

    public required string PaisId { get; set; }
    public required Pais Pais { get; set; }

    public required string ProvinciaId { get; set; }
    public required Provincia Provincia { get; set; }

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];
}
