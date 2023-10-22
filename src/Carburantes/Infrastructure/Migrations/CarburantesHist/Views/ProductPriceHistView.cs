namespace Seedysoft.Carburantes.Infrastructure.Migrations.CarburantesHist.Views;

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
            _ => throw InfrastructureLib.Migrations.Shared.Constants.UnknownVersionNumberApplicationException
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
            _ => throw InfrastructureLib.Migrations.Shared.Constants.UnknownVersionNumberApplicationException
        };
    }

    public static string GetDropSql() => $@"DROP VIEW IF EXISTS {ViewName}";

    // Uncoment when next version
    //private static string Version20230222 => $@"CREATE VIEW {ViewName} AS ......";

    private static string Version20221230 => $@"CREATE VIEW {ViewName} AS 
        SELECT
            {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.AtDate)}
          , {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.CentimosDeEuro)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.CodigoPostal)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Direccion)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Horario)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.IdEstacion)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Latitud)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Localidad)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.LongitudWgs84)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Margen)}
          , {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.Rotulo)}
          , {nameof(Core.Entities.ProductoPetroliferoHist)}.{nameof(Core.Entities.ProductoPetroliferoHist.IdProducto)}
          , {nameof(Core.Entities.ProductoPetroliferoHist)}.{nameof(Core.Entities.ProductoPetroliferoHist.NombreProducto)}
          , {nameof(Core.Entities.ProductoPetroliferoHist)}.{nameof(Core.Entities.ProductoPetroliferoHist.NombreProductoAbreviatura)}
          , {nameof(Core.Entities.MunicipioHist)}.{nameof(Core.Entities.MunicipioHist.NombreMunicipio)}
          , {nameof(Core.Entities.ProvinciaHist)}.{nameof(Core.Entities.ProvinciaHist.NombreProvincia)}
        FROM {nameof(Core.Entities.EstacionProductoPrecioHist)}
          INNER JOIN {nameof(Core.Entities.EstacionServicioHist)} ON {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.IdEstacion)} = {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.IdEstacion)} AND {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.AtDate)} = {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.AtDate)}
          INNER JOIN {nameof(Core.Entities.ProductoPetroliferoHist)} ON {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.IdProducto)} = {nameof(Core.Entities.ProductoPetroliferoHist)}.{nameof(Core.Entities.ProductoPetroliferoHist.IdProducto)} AND {nameof(Core.Entities.EstacionProductoPrecioHist)}.{nameof(Core.Entities.EstacionProductoPrecioHist.AtDate)} = {nameof(Core.Entities.ProductoPetroliferoHist)}.{nameof(Core.Entities.ProductoPetroliferoHist.AtDate)}
          INNER JOIN {nameof(Core.Entities.MunicipioHist)} ON {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.IdMunicipio)} = {nameof(Core.Entities.MunicipioHist)}.{nameof(Core.Entities.MunicipioHist.IdMunicipio)} AND {nameof(Core.Entities.EstacionServicioHist)}.{nameof(Core.Entities.EstacionServicioHist.AtDate)} = {nameof(Core.Entities.MunicipioHist)}.{nameof(Core.Entities.MunicipioHist.AtDate)}
          INNER JOIN {nameof(Core.Entities.ProvinciaHist)} ON {nameof(Core.Entities.MunicipioHist)}.{nameof(Core.Entities.MunicipioHist.IdProvincia)} = {nameof(Core.Entities.ProvinciaHist)}.{nameof(Core.Entities.ProvinciaHist.IdProvincia)} AND {nameof(Core.Entities.ProvinciaHist)}.{nameof(Core.Entities.ProvinciaHist.AtDate)} = {nameof(Core.Entities.MunicipioHist)}.{nameof(Core.Entities.MunicipioHist.AtDate)}
        ";
}
