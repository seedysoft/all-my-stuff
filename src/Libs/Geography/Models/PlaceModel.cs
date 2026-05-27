namespace Seedysoft.Libs.Geography.Models;

/// <summary>
/// Represents a geocoding result with identifiers, administrative divisions, address details and geographic coordinates.
/// </summary>
public readonly record struct PlaceModel
{
    /// <summary>
    /// Identificador de la referencia.
    /// </summary>
    [J("id")] public required string Id { get; init; }
    /// <summary>
    /// Provincia a la que pertenece (si corresponde).
    /// </summary>
    [J("province")] public string? Province { get; init; }
    /// <summary>
    /// Código de la provincia a la que pertenece.
    /// </summary>
    [J("provinceCode")] public string? ProvinceCode { get; init; }
    /// <summary>
    /// Comunidad Autónoma a la que pertenece (si corresponde).
    /// </summary>
    [J("comunidadAutonoma")] public string? ComunidadAutonoma { get; init; }
    /// <summary>
    /// Código de la Comunidad Autónoma a la que pertenece.
    /// </summary>
    [J("comunidadAutonomaCode")] public string? ComunidadAutonomaCode { get; init; }
    /// <summary>
    /// Municipio al que pertenece (si corresponde al tipo de entidad).
    /// </summary>
    [J("muni")] public string? Muni { get; init; }
    /// <summary>
    /// Código del municipio.
    /// </summary>
    [J("muniCode")] public string? MuniCode { get; init; }
    /// <summary>
    /// Tipo de entidad. Los valores pueden ser 'callejero' (viales urbanos), 'portal' (portal o punto kilométrico), 'carreteras' (viales interurbanos), 'Codpost' (código postal), 'municipio', 'provincia', 'comunidad autonoma', 'toponimo', 'punto_recarga_electrica', 'ngbe' y 'refcatastral'.
    /// </summary>
    [J("type")] public string? Type { get; init; }
    /// <summary>
    /// Texto completo del nombre de los resultados.
    /// </summary>
    [J("address")] public string? Address { get; init; }
    /// <summary>
    /// Código postal (si corresponde).
    /// </summary>
    [J("postalCode")] public string? PostalCode { get; init; }
    /// <summary>
    /// Población a la que pertenece (si corresponde).
    /// </summary>
    [J("poblacion")] public string? Poblacion { get; init; }
    /// <summary>
    /// no disponible con esta petición. 
    /// </summary>
    [J("geom")] public object? Geom { get; init; }
    /// <summary>
    /// Especifica el tipo de vía.
    /// </summary>
    [J("tip_via")] public string? TipVia { get; init; }
    /// <summary>
    /// Coordenada que representa la latitud de la entidad de los elementos puntuales (portales, puntos kilométricos, puntos de interés y topónimos).
    /// </summary>
    [J("lat")] public float Lat { get; init; }
    /// <summary>
    /// Coordenada que representa la longitud de la entidad de los elementos puntuales (portales, puntos kilométricos, puntos de interés y topónimos). 
    /// </summary>
    [J("lng")] public float Lng { get; init; }
    /// <summary>
    /// Número de portal o punto kilométrico (si se especifica en la consulta).
    /// </summary>
    [J("portalNumber")] public int? PortalNumber { get; init; }
    /// <summary>
    /// su valor puede ser “true” cuando el portal encontrado tiene como número S-N, o “false” cuando se esté buscando un número de portal distinto a S-N.
    /// </summary>
    [J("noNumber")] public bool? NoNumber { get; init; }
    /// <summary>
    /// Vacío (este valor con la versión actual del geocoder, se ha suprimido, ya que se emplea elasticsearch y no se puede configurar la salida candidates según grado de coincidencia).
    /// </summary>
    [J("stateMsg")] public string? StateMsg { get; init; }
    /// <summary>
    /// Extensión del número del portal.
    /// </summary>
    [J("extension")] public object? Extension { get; init; }
    /// <summary>
    /// 0 (este valor con la versión actual del geocoder, se ha suprimido, ya que se emplea elasticsearch y no se puede configurar la salida de candidates según grado de coincidencia).
    /// </summary>
    [J("state")] public int State { get; init; }
    /// <summary>
    /// Código del país (por defecto '011' para España).
    /// </summary>
    [J("countryCode")] public string? CountryCode { get; init; }
    /// <summary>
    /// Referencia catastral (si corresponde).
    /// </summary>
    [J("refCatastral")] public string? RefCatastral { get; init; }
}
