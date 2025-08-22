namespace Seedysoft.Libs.Core.Entities;

public class Ministerio
{
    public int MinisterioId { get; set; }

    public required string MinisterioDenominacion { get; set; }

    //public virtual ICollection<Puesto> Puesto { get; set; } = [];
}
