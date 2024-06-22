using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Migrations.Views;

public static class ProductPriceView
{
    // View version numbers
    public enum VersionNumbers
    {
        // Uncoment when next version
        //Version20230222,
        Version20221230
    }

    private const string ViewName = nameof(ProductPriceView);

    // Get the previous view create statement for a version number
    public static string GetCreateSqlPrevious(VersionNumbers versionNumber)
    {
        return versionNumber switch
        {
            // Uncoment when next version
            //VersionNumber.Version20230222 => Version20230222,
            VersionNumbers.Version20221230 => GetDropSql(),
            _ => throw Libs.Infrastructure.Migrations.Shared.Constants.UnknownVersionNumberApplicationException
        };
    }

    // Get the view create statement for the version number
    public static string GetCreateSql(VersionNumbers versionNumber)
    {
        return versionNumber switch
        {
            // Uncoment when next version
            //VersionNumber.Version20230222 => Version20230222,
            VersionNumbers.Version20221230 => Version20221230,
            _ => throw Libs.Infrastructure.Migrations.Shared.Constants.UnknownVersionNumberApplicationException
        };
    }

    public static string GetDropSql() => $@"DROP VIEW IF EXISTS {ViewName}";

    // Uncoment when next version
    //private static string Version20221230 => $@"CREATE VIEW {ViewName} AS ......";

    private static string Version20221230 => $@"CREATE VIEW {ViewName} AS 
        SELECT
            {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.AtDate)}
          , {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.CentimosDeEuro)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.CodigoPostal)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Direccion)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Horario)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.IdEstacion)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Latitud)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Localidad)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.LongitudWgs84)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Margen)}
          , {nameof(EstacionServicio)}.{nameof(EstacionServicio.Rotulo)}
          , {nameof(ProductoPetrolifero)}.{nameof(ProductoPetrolifero.IdProducto)}
          , {nameof(ProductoPetrolifero)}.{nameof(ProductoPetrolifero.NombreProducto)}
          , {nameof(ProductoPetrolifero)}.{nameof(ProductoPetrolifero.NombreProductoAbreviatura)}
          , {nameof(Municipio)}.{nameof(Municipio.NombreMunicipio)}
          , {nameof(Provincia)}.{nameof(Provincia.NombreProvincia)}
        FROM {nameof(EstacionProductoPrecio)}
          INNER JOIN {nameof(EstacionServicio)} ON {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.IdEstacion)} = {nameof(EstacionServicio)}.{nameof(EstacionServicio.IdEstacion)} AND {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.AtDate)} = {nameof(EstacionServicio)}.{nameof(EstacionServicio.AtDate)}
          INNER JOIN {nameof(ProductoPetrolifero)} ON {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.IdProducto)} = {nameof(ProductoPetrolifero)}.{nameof(ProductoPetrolifero.IdProducto)} AND {nameof(EstacionProductoPrecio)}.{nameof(EstacionProductoPrecio.AtDate)} = {nameof(ProductoPetrolifero)}.{nameof(ProductoPetrolifero.AtDate)}
          INNER JOIN {nameof(Municipio)} ON {nameof(EstacionServicio)}.{nameof(EstacionServicio.IdMunicipio)} = {nameof(Municipio)}.{nameof(Municipio.IdMunicipio)} AND {nameof(EstacionServicio)}.{nameof(EstacionServicio.AtDate)} = {nameof(Municipio)}.{nameof(Municipio.AtDate)}
          INNER JOIN {nameof(Provincia)} ON {nameof(Municipio)}.{nameof(Municipio.IdProvincia)} = {nameof(Provincia)}.{nameof(Provincia.IdProvincia)} AND {nameof(Provincia)}.{nameof(Provincia.AtDate)} = {nameof(Municipio)}.{nameof(Municipio.AtDate)}
        ";
}
