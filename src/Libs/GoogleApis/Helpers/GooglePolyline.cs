namespace Seedysoft.Libs.GoogleApis.Helpers;

/// <summary>
/// See https://developers.google.com/maps/documentation/utilities/polylinealgorithm
/// </summary>
public static class GooglePolyline
{
    /// <summary>
    /// Decode google style polyline coordinates.
    /// </summary>
    /// <param name="encodedPoints"></param>
    /// <returns></returns>
    public static IEnumerable<Models.Shared.LatLngLiteral> Decode(string encodedPoints)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encodedPoints, nameof(encodedPoints));

        char[] polylineChars = encodedPoints.ToCharArray();
        int index = 0;

        int currentLat = 0;
        int currentLng = 0;
        int next5bits;
        int sum;
        int shifter;

        while (index < polylineChars.Length)
        {
            // calculate next latitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length)
                break;

            currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            //calculate next longitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length && next5bits >= 32)
                break;

            currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            yield return new Models.Shared.LatLngLiteral
            {
                Lat = Convert.ToDouble(currentLat) / 1E5,
                Lng = Convert.ToDouble(currentLng) / 1E5
            };
        }
    }

    /// <summary>
    /// Encode it
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static string Encode(IEnumerable<Models.Shared.LatLngLiteral> points)
    {
        System.Text.StringBuilder str = new();

        int lastLat = 0;
        int lastLng = 0;

        foreach (Models.Shared.LatLngLiteral point in points)
        {
            int lat = (int)Math.Round(point.Lat * 1E5);
            int lng = (int)Math.Round(point.Lng * 1E5);

            encodeDiff(lat - lastLat, str);
            encodeDiff(lng - lastLng, str);

            lastLat = lat;
            lastLng = lng;
        }

        return str.ToString();

        static void encodeDiff(int diff, System.Text.StringBuilder str)
        {
            int shifted = diff << 1;
            if (diff < 0)
                shifted = ~shifted;

            int rem = shifted;

            while (rem >= 0x20)
            {
                _ = str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                rem >>= 5;
            }

            _ = str.Append((char)(rem + 63));
        }
    }
}
