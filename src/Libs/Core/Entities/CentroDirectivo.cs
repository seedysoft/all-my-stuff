namespace Seedysoft.Libs.Core.Entities;

public class CentroDirectivo
{
    public int CentroDirectivoId { get; set; }

    public required string CentroDirectivoDenominacion { get; set; }

    //public virtual ICollection<Puesto> Puesto { get; set; } = [];
}
