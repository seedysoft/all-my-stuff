namespace Seedysoft.Libs.Core.Entities;

public class Localidad
{
    public required string LocalidadId { get; set; }

    public required string LocalidadDenominacion { get; set; }

    //public virtual ICollection<Puesto> PuestoLocalidad { get; set; } = [];

    //public virtual ICollection<Puesto> PuestoResidenciaLocalidad { get; set; } = [];
}
