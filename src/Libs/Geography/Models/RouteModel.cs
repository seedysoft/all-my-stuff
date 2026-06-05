namespace Seedysoft.Libs.Geography.Models;

/// <summary>
/// Represents a routing result containing bounding box, total distance, estimated time, encoded geometry, origin and destination coordinates, computation info, and navigation instructions.
/// </summary>
/// <remarks>
/// Distance is expressed in meters.
/// Time values represent milliseconds.
/// Geometry is encoded using Google's polyline format. 
/// 'From' is the snapped start location (e.g., nearest entrance or kilometer point).
/// 'Found' indicates whether a route was located.
/// Estimated time may be unreliable for cars if one-way data is missing.
/// </remarks>
public record RouteModel
{
    /// <summary>
    /// Bounding box o recuadro mínimo envolvente de la distancia.
    /// </summary>
    [J("bbox")] public string[]? Bbox { get; init; }
    /// <summary>
    /// Distancia total en metros.
    /// </summary>
    [J("distance")] public string? Distance { get; init; }
    /// <summary>
    /// true si se ha encontrado un camino.
    /// </summary>
    [J("found")] public string? Found { get; init; }
    /// <summary>
    /// Ubicación del punto real que se ha tomado como inicio de trazado (ya que ésta comienza en el portal de CartoCiudad o punto kilométrico más próximo).
    /// </summary>
    [J("from")] public string? From { get; init; }
    /// <summary>
    /// Geometría del trazado, siguiendo el formato comprimido de Google.
    /// </summary>
    [J("geom")] public string? Geom { get; init; }
    /// <summary>
    /// Tiempos que se han empleado en el cálculo (milisegundos).
    /// </summary>
    [J("info")] public Info? Info { get; init; }
    /// <summary>
    /// Contiene el listado de instrucciones que componen la distancia.
    /// </summary>
    [J("instructionsData")] public InstructionData? InstructionsData { get; init; }
    /// <summary>
    /// Tiempo estimado (no es fiable para coches al no disponer de datos sobre los sentidos de circulación).
    /// </summary>
    [J("time")] public string? Time { get; init; }
    /// <summary>
    /// Coordenadas del punto destino.
    /// </summary>
    [J("to")] public string? To { get; init; }
}

public record Info
{
    [J("routeFound")] public string? RouteFound { get; init; }
    [J("took")] public string? Took { get; init; }
    [J("tookGeocoding")] public string? TookGeocoding { get; init; }
}

public record InstructionData
{
    [J("instruction")] public Instruction[]? Instructions { get; init; }
}

public record Instruction
{
    /// <summary>
    /// Bounding box del segmento de trazado para poder hacer zoom al mismo.
    /// </summary>
    [J("bbox")] public string[]? Bbox { get; init; }
    /// <summary>
    /// Mensajes acerca de las calles por dónde debe pasar.
    /// </summary>
    [J("description")] public string? Description { get; init; }
    /// <summary>
    /// Destino del segmento de distancia.
    /// </summary>
    [J("dest")] public string[]? Dest { get; init; }
    /// <summary>
    /// Distancia en metros del segmento de distancia.
    /// </summary>
    [J("distance")] public string? Distance { get; init; }
    /// <summary>
    /// Entero que indica si hay que girar, seguir recto, etc.
    /// </summary>
    [J("indication")] public string? Indication { get; init; }
    /// <summary>
    /// Origen del segmento de distancia.
    /// </summary>
    [J("orig")] public string[]? Orig { get; init; }
}
