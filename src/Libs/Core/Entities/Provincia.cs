namespace Seedysoft.Libs.Core.Entities;

public class Provincia
{
    public required int ProvinciaId { get; set; }

    public required string ProvinciaDenominacion { get; set; }

    public required int PaisId { get; set; }
    public Pais Pais { get; set; } = default!;

    //public virtual ICollection<Localidad> Localidades { get; set; } = [];

    //public virtual ICollection<Puesto> Puestos { get; set; } = [];

    //public virtual ICollection<Unidad> Unidades { get; set; } = [];
}
