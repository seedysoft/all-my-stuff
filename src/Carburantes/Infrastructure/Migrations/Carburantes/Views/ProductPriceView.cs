namespace Seedysoft.Carburantes.Infrastructure.Migrations.Carburantes.Views;

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
    //private static string Version20221230 => $@"CREATE VIEW {ViewName} AS ......";

    private static string Version20221230 => $@"CREATE VIEW {ViewName} AS 
        SELECT
            {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.AtDate)}
          , {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.CentimosDeEuro)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.CodigoPostal)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Direccion)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Horario)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.IdEstacion)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Latitud)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Localidad)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.LongitudWgs84)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Margen)}
          , {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.Rotulo)}
          , {nameof(Core.Entities.ProductoPetrolifero)}.{nameof(Core.Entities.ProductoPetrolifero.IdProducto)}
          , {nameof(Core.Entities.ProductoPetrolifero)}.{nameof(Core.Entities.ProductoPetrolifero.NombreProducto)}
          , {nameof(Core.Entities.ProductoPetrolifero)}.{nameof(Core.Entities.ProductoPetrolifero.NombreProductoAbreviatura)}
          , {nameof(Core.Entities.Municipio)}.{nameof(Core.Entities.Municipio.NombreMunicipio)}
          , {nameof(Core.Entities.Provincia)}.{nameof(Core.Entities.Provincia.NombreProvincia)}
        FROM {nameof(Core.Entities.EstacionProductoPrecio)}
          INNER JOIN {nameof(Core.Entities.EstacionServicio)} ON {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.IdEstacion)} = {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.IdEstacion)} AND {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.AtDate)} = {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.AtDate)}
          INNER JOIN {nameof(Core.Entities.ProductoPetrolifero)} ON {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.IdProducto)} = {nameof(Core.Entities.ProductoPetrolifero)}.{nameof(Core.Entities.ProductoPetrolifero.IdProducto)} AND {nameof(Core.Entities.EstacionProductoPrecio)}.{nameof(Core.Entities.EstacionProductoPrecio.AtDate)} = {nameof(Core.Entities.ProductoPetrolifero)}.{nameof(Core.Entities.ProductoPetrolifero.AtDate)}
          INNER JOIN {nameof(Core.Entities.Municipio)} ON {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.IdMunicipio)} = {nameof(Core.Entities.Municipio)}.{nameof(Core.Entities.Municipio.IdMunicipio)} AND {nameof(Core.Entities.EstacionServicio)}.{nameof(Core.Entities.EstacionServicio.AtDate)} = {nameof(Core.Entities.Municipio)}.{nameof(Core.Entities.Municipio.AtDate)}
          INNER JOIN {nameof(Core.Entities.Provincia)} ON {nameof(Core.Entities.Municipio)}.{nameof(Core.Entities.Municipio.IdProvincia)} = {nameof(Core.Entities.Provincia)}.{nameof(Core.Entities.Provincia.IdProvincia)} AND {nameof(Core.Entities.Provincia)}.{nameof(Core.Entities.Provincia.AtDate)} = {nameof(Core.Entities.Municipio)}.{nameof(Core.Entities.Municipio.AtDate)}
        ";
}
