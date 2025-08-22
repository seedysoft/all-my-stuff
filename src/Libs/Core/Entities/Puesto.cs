namespace Seedysoft.Libs.Core.Entities;

public class Puesto
{
    public int PuestoId { get; set; }

    public required string PuestoDenominacionCorta { get; set; }

    public required string PuestoDenominacionLarga { get; set; }

    public int Nivel { get; set; }

    public required decimal ComplementoEspecifico { get; set; }

    //public TiposPuesto? TipoPuesto { get; set; }
    public string? TipoPuesto { get; set; }

    //public FormasProvision? Provision { get; set; }
    public string? Provision { get; set; }

    //public AdscripcionesAdministracion? Adscripcion { get; set; }
    public string? Adscripcion { get; set; }

    //public Grupos? GrupoSubgrupo { get; set; }
    public string? GrupoSubgrupo { get; set; }

    //public AdscripcionesCuerpos? Cuerpo { get; set; }
    public string? Cuerpo { get; set; }

    public string? TitulacionAcademica { get; set; }

    public string? FormacionEspecifica { get; set; }

    public string? Observaciones { get; set; }

    //public required Estados Estado { get; set; }
    public required string Estado { get; set; }

    //public int MinisterioId { get; set; }
    //public required Ministerio Ministerio { get; set; }

    //public int CentroDirectivoId { get; set; }
    //public required CentroDirectivo CentroDirectivo { get; set; }

    public int UnidadId { get; set; }
    public required Unidad Unidad { get; set; }

    public string? PaisId { get; set; }
    public Pais? Pais { get; set; }

    public string? ProvinciaId { get; set; }
    public Provincia? Provincia { get; set; }

    public string? LocalidadId { get; set; }
    public Localidad? Localidad { get; set; }
}
