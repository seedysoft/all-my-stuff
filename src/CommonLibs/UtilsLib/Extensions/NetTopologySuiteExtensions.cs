using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace Seedysoft.UtilsLib.Extensions;

public static class NetTopologySuiteExtensions
{
    public static readonly CoordinateSystemServices OurCoordinateSystemServices = new(
        new Dictionary<int, string>
        {
            // Coordinate systems:

            [Constants.CoordinateSystemCodes.Wgs84] =
            GeographicCoordinateSystem.WGS84.WKT,

            [Constants.CoordinateSystemCodes.Epsg3857] =
            @"
                    PROJCS[""WGS 84 / Pseudo-Mercator"",
                        GEOGCS[""WGS 84"",
                            DATUM[""WGS_1984"",
                                SPHEROID[""WGS 84"",6378137,298.257223563,
                                    AUTHORITY[""EPSG"",""7030""]],
                                AUTHORITY[""EPSG"",""6326""]],
                            PRIMEM[""Greenwich"",0,
                                AUTHORITY[""EPSG"",""8901""]],
                            UNIT[""degree"",0.0174532925199433,
                                AUTHORITY[""EPSG"",""9122""]],
                            AUTHORITY[""EPSG"",""4326""]],
                        PROJECTION[""Mercator_1SP""],
                        PARAMETER[""central_meridian"",0],
                        PARAMETER[""scale_factor"",1],
                        PARAMETER[""false_easting"",0],
                        PARAMETER[""false_northing"",0],
                        UNIT[""metre"",1,
                            AUTHORITY[""EPSG"",""9001""]],
                        AXIS[""X"",EAST],
                        AXIS[""Y"",NORTH],
                        EXTENSION[""PROJ4"",""+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext  +no_defs""],
                        AUTHORITY[""EPSG"",""3857""]]
                ",

            /*
            // This coordinate system covers the area of our data.
            // Different data requires a different coordinate system.
            [2855] =
            @"
                PROJCS[""NAD83(HARN) / Washington North"",
                    GEOGCS[""NAD83(HARN)"",
                        DATUM[""NAD83_High_Accuracy_Regional_Network"",
                            SPHEROID[""GRS 1980"",6378137,298.257222101,
                                AUTHORITY[""EPSG"",""7019""]],
                            AUTHORITY[""EPSG"",""6152""]],
                        PRIMEM[""Greenwich"",0,
                            AUTHORITY[""EPSG"",""8901""]],
                        UNIT[""degree"",0.01745329251994328,
                            AUTHORITY[""EPSG"",""9122""]],
                        AUTHORITY[""EPSG"",""4152""]],
                    PROJECTION[""Lambert_Conformal_Conic_2SP""],
                    PARAMETER[""standard_parallel_1"",48.73333333333333],
                    PARAMETER[""standard_parallel_2"",47.5],
                    PARAMETER[""latitude_of_origin"",47],
                    PARAMETER[""central_meridian"",-120.8333333333333],
                    PARAMETER[""false_easting"",500000],
                    PARAMETER[""false_northing"",0],
                    UNIT[""metre"",1,
                        AUTHORITY[""EPSG"",""9001""]],
                    AUTHORITY[""EPSG"",""2855""]]
            ",
            */
        });

    public static T ProjectTo<T>(this T geometry, int srid) where T : Geometry
    {
        ICoordinateTransformation CoordinateTransformation = OurCoordinateSystemServices.CreateTransformation(geometry.SRID, srid);

        var GeometryCopy = (T)geometry.Copy();

        GeometryCopy.Apply(new MathTransformFilter(CoordinateTransformation.MathTransform));

        return GeometryCopy;
    }

    private class MathTransformFilter : ICoordinateSequenceFilter
    {
        private readonly MathTransform _transform;

        public MathTransformFilter(MathTransform transform) => _transform = transform;

        public bool Done => false;
        public bool GeometryChanged => true;

        public void Filter(CoordinateSequence coordinateSequence, int i)
        {
            double x = coordinateSequence.GetX(i);
            double y = coordinateSequence.GetY(i);
            double z = coordinateSequence.GetZ(i);

            _transform.Transform(ref x, ref y, ref z);

            coordinateSequence.SetX(i, x);
            coordinateSequence.SetY(i, y);
            coordinateSequence.SetZ(i, z);
        }
    }
}