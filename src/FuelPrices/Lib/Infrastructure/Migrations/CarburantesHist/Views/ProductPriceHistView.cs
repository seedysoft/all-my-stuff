using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Migrations.CarburantesHist.Views;

public static class ProductPriceHistView
{
    // View version numbers
    public enum VersionNumbers
    {
        // Uncoment when next version
        //Version20230222,
        Version20221230
    }
    private const string ViewName = nameof(ProductPriceHistView);

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
    //private static string Version20230222 => $@"CREATE VIEW {ViewName} AS ......";

    private static string Version20221230 => $@"CREATE VIEW {ViewName} AS 
        SELECT
            {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.AtDate)}
          , {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.CentimosDeEuro)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.CodigoPostal)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Direccion)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Horario)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.IdEstacion)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Latitud)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Localidad)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.LongitudWgs84)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Margen)}
          , {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.Rotulo)}
          , {nameof(ProductoPetroliferoHist)}.{nameof(ProductoPetroliferoHist.IdProducto)}
          , {nameof(ProductoPetroliferoHist)}.{nameof(ProductoPetroliferoHist.NombreProducto)}
          , {nameof(ProductoPetroliferoHist)}.{nameof(ProductoPetroliferoHist.NombreProductoAbreviatura)}
          , {nameof(MunicipioHist)}.{nameof(MunicipioHist.NombreMunicipio)}
          , {nameof(ProvinciaHist)}.{nameof(ProvinciaHist.NombreProvincia)}
        FROM {nameof(EstacionProductoPrecioHist)}
          INNER JOIN {nameof(EstacionServicioHist)} ON {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.IdEstacion)} = {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.IdEstacion)} AND {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.AtDate)} = {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.AtDate)}
          INNER JOIN {nameof(ProductoPetroliferoHist)} ON {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.IdProducto)} = {nameof(ProductoPetroliferoHist)}.{nameof(ProductoPetroliferoHist.IdProducto)} AND {nameof(EstacionProductoPrecioHist)}.{nameof(EstacionProductoPrecioHist.AtDate)} = {nameof(ProductoPetroliferoHist)}.{nameof(ProductoPetroliferoHist.AtDate)}
          INNER JOIN {nameof(MunicipioHist)} ON {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.IdMunicipio)} = {nameof(MunicipioHist)}.{nameof(MunicipioHist.IdMunicipio)} AND {nameof(EstacionServicioHist)}.{nameof(EstacionServicioHist.AtDate)} = {nameof(MunicipioHist)}.{nameof(MunicipioHist.AtDate)}
          INNER JOIN {nameof(ProvinciaHist)} ON {nameof(MunicipioHist)}.{nameof(MunicipioHist.IdProvincia)} = {nameof(ProvinciaHist)}.{nameof(ProvinciaHist.IdProvincia)} AND {nameof(ProvinciaHist)}.{nameof(ProvinciaHist.AtDate)} = {nameof(MunicipioHist)}.{nameof(MunicipioHist.AtDate)}
        ";
}
