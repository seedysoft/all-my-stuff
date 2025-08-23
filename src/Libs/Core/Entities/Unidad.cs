namespace Seedysoft.Libs.Core.Entities;

public class Unidad
{
    public int UnidadId { get; set; }

    public required string UnidadDenominacion { get; set; }

    public required int CentroDirectivoId { get; set; }
    public CentroDirectivo CentroDirectivo { get; set; } = default!;

    public required int PaisId { get; set; }
    public Pais Pais { get; set; } = default!;

    public required int ProvinciaId { get; set; }
    public Provincia Provincia { get; set; } = default!;

    public required int LocalidadId { get; set; }
    public Localidad Localidad { get; set; } = default!;

    //public virtual ICollection<Puesto> Puestos{ get; set; } = [];
}
