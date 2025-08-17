namespace Seedysoft.Libs.Core.Entities;

public class Puesto
{
    public required int PuestoId { get; set; }

    public required string PuestoDenominacionCorta { get; set; }

    public required string PuestoDenominacionLarga { get; set; }

    public byte Nivel { get; set; }

    public decimal ComplementoEspecifico { get; set; }

    public TiposPuesto? TipoPuesto { get; set; }

    public FormasProvision? Provision { get; set; }

    public AdscripcionesAdministracion? Adscripcion { get; set; }

    public Grupos? GrupoSubgrupo { get; set; }

    public AdscripcionesCuerpos? Cuerpo { get; set; }

    public string? TitulacionAcademica { get; set; }

    public string? FormacionEspecifica { get; set; }

    public string? Observaciones { get; set; }

    public required Estados Estado { get; set; }

    public required Ministerio Ministerio { get; set; }

    public required CentroDirectivo CentroDirectivo { get; set; }

    public required Unidad Unidad { get; set; }

    public required Pais Pais { get; set; }

    public required Provincia Provincia { get; set; }

    public required Localidad Localidad { get; set; }

    public Pais? PaisResidencia { get; set; }

    public Provincia? ProvinciaResidencia { get; set; }

    public Localidad? LocalidadResidencia { get; set; }
}
