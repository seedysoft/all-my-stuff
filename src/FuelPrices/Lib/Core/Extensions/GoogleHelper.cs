namespace Seedysoft.FuelPrices.Lib.Core.Extensions;

public sealed class GoogleHelper
{
    private GoogleHelper() { }

    /// <summary>
    /// Google Polyline Conversion Const
    /// </summary>
    private const double GooglePolylineConversionConst = 1E5; // = 100000.0; // 10 to the 5th power (the e meaning 'exponent')

    /// <summary>
    /// Decode google style polyline coordinates.
    /// </summary>
    /// <param name="encodedPoints"></param>
    /// <returns></returns>
    public static IEnumerable<JsonObjects.GoogleMaps.LocationJson> Decode(string encodedPoints)
    {
        if (string.IsNullOrEmpty(encodedPoints))
            throw new ArgumentNullException(nameof(encodedPoints));

        char[] PolylineChars = encodedPoints.ToCharArray();
        int PolylineCharsIndex = 0;

        int CurrentLat = 0;
        int CurrentLng = 0;
        int Sum;
        int Shifter;
        int Next5bits;

        while (PolylineCharsIndex < PolylineChars.Length)
        {
            // calculate next latitude
            Sum = 0;
            Shifter = 0;
            do
            {
                Next5bits = PolylineChars[PolylineCharsIndex++] - 63;
                Sum |= (Next5bits & 31) << Shifter;
                Shifter += 5;
            } while (Next5bits >= 32 && PolylineCharsIndex < PolylineChars.Length);

            if (PolylineCharsIndex >= PolylineChars.Length)
                break;

            CurrentLat += (Sum & 1) == 1 ? ~(Sum >> 1) : Sum >> 1;

            //calculate next longitude
            Sum = 0;
            Shifter = 0;
            do
            {
                Next5bits = PolylineChars[PolylineCharsIndex++] - 63;
                Sum |= (Next5bits & 31) << Shifter;
                Shifter += 5;
            } while (Next5bits >= 32 && PolylineCharsIndex < PolylineChars.Length);

            if (PolylineCharsIndex >= PolylineChars.Length && Next5bits >= 32)
                break;

            CurrentLng += (Sum & 1) == 1 ? ~(Sum >> 1) : Sum >> 1;

            yield return new JsonObjects.GoogleMaps.LocationJson(
                lat: Convert.ToDouble(CurrentLat) / GooglePolylineConversionConst,
                lng: Convert.ToDouble(CurrentLng) / GooglePolylineConversionConst);
        }
    }
}
