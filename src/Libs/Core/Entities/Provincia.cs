namespace Seedysoft.Libs.Core.Entities;

public class Provincia
{
    public required string ProvinciaId { get; set; }

    public required string ProvinciaDenominacion { get; set; }

    public required string PaisId { get; set; }
    public required Pais Pais { get; set; }

    //public virtual ICollection<Localidad> Localidades { get; set; } = [];

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];
}
