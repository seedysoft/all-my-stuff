namespace Seedysoft.BlazorWebApp.Client.Constants;

public sealed class TravelUris : BaseUris
{
    public const string Controller = $"{Api}travel";

    public sealed class Actions
    {
        public const string FindPlaces = nameof(FindPlaces);
        public const string GetGasStations = nameof(GetGasStations);
        //public const string ObtainGasStations = nameof(ObtainGasStations);

        private Actions() { }
    }

    private TravelUris() { }
}
