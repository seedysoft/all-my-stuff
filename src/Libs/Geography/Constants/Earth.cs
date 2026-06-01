namespace Seedysoft.Libs.Geography.Constants;

public static class Earth
{
    /// <summary>
    /// 
    /// </summary>
    public readonly record struct Home
    {
        private const double Lng = -3.6628374207940815d;
        private const double Lat = 42.354397413084406d;

        public Home() { }

        public static NetTopologySuite.Geometries.Point Center { get; } = new(Lng, Lat) { SRID = (int)ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84.AuthorityCode };

        //public static void Asdfassdfasdf()
        //{
        //    // EPSG:4326 (WGS84 Lat/Lon)
        //    GeographicCoordinateSystem Wgs84GeographicCoordinateSystem = GeographicCoordinateSystem.WGS84;

        //    // EPSG:3857 (Web Mercator)
        //    ProjectedCoordinateSystem WebMercatorProjectedCoordinateSystem = ProjectedCoordinateSystem.WebMercator;

        //    // Create transformation
        //    CoordinateTransformationFactory coordinateTransformationFactory = new();
        //    ICoordinateTransformation CoordinateTransformation = coordinateTransformationFactory.CreateFromCoordinateSystems(Wgs84GeographicCoordinateSystem, WebMercatorProjectedCoordinateSystem);

        //    double[] mercator = CoordinateTransformation.MathTransform.Transform([Lng, Lat]);

        //    Console.WriteLine($"Lat/Lon: ({Lat}, {Lng})");
        //    Console.WriteLine($"EPSG:3857: X={mercator[0]:F2}, Y={mercator[1]:F2}");
        //}
    }
}
