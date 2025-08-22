namespace Seedysoft.Libs.Core.Entities;

public class Unidad
{
    public int UnidadId { get; set; }

    public required string UnidadDenominacion { get; set; }

    public int CentroDirectivoId { get; set; }
    public required CentroDirectivo CentroDirectivo { get; set; }

    public required string PaisId { get; set; }
    public required Pais Pais { get; set; }

    public required string ProvinciaId { get; set; }
    public required Provincia Provincia { get; set; }

    public required string? LocalidadId { get; set; }
    public required Localidad Localidad { get; set; }

    //public virtual ICollection<Puesto> Puestos{ get; set; } = [];
}
