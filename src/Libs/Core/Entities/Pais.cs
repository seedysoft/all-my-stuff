namespace Seedysoft.Libs.Core.Entities;

public class Pais
{
    public required string PaisId { get; set; }

    public required string PaisDenominacion { get; set; }

    //public virtual ICollection<Puesto> PuestoPais { get; set; } = [];

    //public virtual ICollection<Puesto> PuestoResidenciaPais { get; set; } = [];
}
