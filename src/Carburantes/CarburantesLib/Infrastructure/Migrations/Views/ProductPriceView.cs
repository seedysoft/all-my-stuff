namespace Seedysoft.CarburantesLib.Infrastructure.Migrations.Views;

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
            {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.AtDate)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.CentimosDeEuro)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.CodigoPostal)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Direccion)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Horario)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.IdEstacion)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Latitud)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Localidad)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.LongitudWgs84)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Margen)}
          , {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.Rotulo)}
          , {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)}.{nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero.IdProducto)}
          , {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)}.{nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero.NombreProducto)}
          , {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)}.{nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero.NombreProductoAbreviatura)}
          , {nameof(Carburantes.CoreLib.Entities.Municipio)}.{nameof(Carburantes.CoreLib.Entities.Municipio.NombreMunicipio)}
          , {nameof(Carburantes.CoreLib.Entities.Provincia)}.{nameof(Carburantes.CoreLib.Entities.Provincia.NombreProvincia)}
        FROM {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}
          INNER JOIN {nameof(Carburantes.CoreLib.Entities.EstacionServicio)} ON {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.IdEstacion)} = {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.IdEstacion)} AND {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.AtDate)} = {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.AtDate)}
          INNER JOIN {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)} ON {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.IdProducto)} = {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)}.{nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero.IdProducto)} AND {nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio)}.{nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio.AtDate)} = {nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero)}.{nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero.AtDate)}
          INNER JOIN {nameof(Carburantes.CoreLib.Entities.Municipio)} ON {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.IdMunicipio)} = {nameof(Carburantes.CoreLib.Entities.Municipio)}.{nameof(Carburantes.CoreLib.Entities.Municipio.IdMunicipio)} AND {nameof(Carburantes.CoreLib.Entities.EstacionServicio)}.{nameof(Carburantes.CoreLib.Entities.EstacionServicio.AtDate)} = {nameof(Carburantes.CoreLib.Entities.Municipio)}.{nameof(Carburantes.CoreLib.Entities.Municipio.AtDate)}
          INNER JOIN {nameof(Carburantes.CoreLib.Entities.Provincia)} ON {nameof(Carburantes.CoreLib.Entities.Municipio)}.{nameof(Carburantes.CoreLib  .Entities.Municipio.IdProvincia)} = {nameof(Carburantes.CoreLib.Entities.Provincia)}.{nameof(Carburantes.CoreLib.Entities.Provincia.IdProvincia)} AND {nameof(Carburantes.CoreLib.Entities.Provincia)}.{nameof(Carburantes.CoreLib.Entities.Provincia.AtDate)} = {nameof(Carburantes.CoreLib.Entities.Municipio)}.{nameof(Carburantes.CoreLib.Entities.Municipio.AtDate)}
        ";
}
