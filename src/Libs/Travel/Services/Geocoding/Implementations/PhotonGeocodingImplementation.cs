using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Geocoding.Implementations;

internal class PhotonGeocodingImplementation(Settings.GeocodingApi api, Microsoft.Extensions.Logging.ILogger logger) : GeocodingImplementationBase(api)
{
    internal override async Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            // photon.komoot.io/api/?q=berlin&limit=1
            RestClient restClient = new(new Uri(Api.UrlFormat).GetLeftPart(UriPartial.Authority));
            ResponseObject? restResponse = await restClient.GetAsync<ResponseObject>(Api.GetUrl(Uri.EscapeDataString(textToFind)), cancellationToken);

            return
                [.. restResponse?.Features?
                    .Select(p => new ViewModels.Place(p.Properties.GetName, p.Geometry.GetLocation)) ?? []
                ];
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    internal record ResponseObject
    {
        //[J("type")] public string Type { get; init; } = default!;
        [J("features")] public Feature[] Features { get; init; } = default!;
    }

    internal record Feature
    {
        //[J("type")] public required string Type { get; init; } = default!;
        [J("properties")] public Properties Properties { get; init; } = default!;
        [J("geometry")] public Geometry Geometry { get; init; } = default!;
    }

    internal record Properties
    {
        //[J("osm_type")] public string OsmType { get; init; } = default!;
        //[J("osm_id")] public long OsmId { get; init; } = default!;
        //[J("osm_key")] public string OsmKey { get; init; } = default!;
        //[J("osm_value")] public string OsmValue { get; init; } = default!;
        [J("type")] public string Type { get; init; } = default!;
        [J("housenumber")] public string? HouseNumber { get; init; } = default!;
        [J("name")] public string? Name { get; init; } = default!;
        [J("street")] public string Street { get; init; } = default!;
        //[J("locality")] public string? Locality { get; init; } = default!;
        //[J("district")] public string? District { get; init; } = default!;
        [J("city")] public string City { get; init; } = default!;
        //[J("county")] public string? County { get; init; } = default!;
        [J("state")] public string? State { get; init; } = default!;
        [J("country")] public string Country { get; init; } = default!;
        //[J("postcode")] public string Postcode { get; init; } = default!;
        //[J("countrycode")] public string Countrycode { get; init; } = default!;
        //[J("extent")] public float[]? Extent { get; init; } = default!;

        [I()]
        public string GetName
        {
            get
            {
                List<string> data = [];

                if (!string.IsNullOrWhiteSpace(Name))
                    data.Add(Name);

                if (!string.IsNullOrWhiteSpace(Street))
                    data.Add(Street);

                if (!string.IsNullOrWhiteSpace(HouseNumber))
                    data.Add(HouseNumber);

                if (!string.IsNullOrWhiteSpace(City))
                    data.Add(City);

                if (!string.IsNullOrWhiteSpace(State))
                    data.Add(State);

                if (!string.IsNullOrWhiteSpace(Country))
                    data.Add(Country);

                return string.Join(", ", data);
            }
        }
    }

    internal record Geometry
    {
        //[J("type")] public string Type { get; init; } = default!;
        [J("coordinates")] public float[] Coordinates { get; init; } = default!;

        [I()] public Models.Location GetLocation => new(Lat: (decimal)Coordinates[1], Lon: (decimal)Coordinates[0]);
    }
}
