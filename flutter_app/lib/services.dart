import 'dart:convert';
import 'dart:math';

import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:http/http.dart' as http;
import 'package:seedysoft_flutter/models.dart';

class GasStationPricesService {
  static const String _baseHost = 'sedeaplicaciones.minetur.gob.es';
  static const String _path =
      '/ServiciosRESTCarburantes/PreciosCarburantes/EstacionesTerrestres';

  final http.Client client;

  GasStationPricesService({http.Client? client})
      : client = client ?? http.Client();

  Future<List<GasStationModel>> getNearGasStations(
    String encodedPolyline,
    int maxDistanceInKm,
    Set<String> selectedProductIds,
  ) async {
    final routePoints = GooglePolyline.decode(encodedPolyline);
    final uri = Uri.https(_baseHost, _path);
    final response = await client.get(uri);
    if (response.statusCode != 200) return [];

    final body = jsonDecode(response.body) as Map<String, dynamic>?;
    if (body == null) return [];

    final stations = (body['ListaEESSPrecio'] as List<dynamic>?) ?? [];
    final parsedStations = <GasStationModel>[];

    for (final raw in stations.cast<Map<String, dynamic>>()) {
      final lat = _parseDouble(raw['Latitud']);
      final lng = _parseDouble(raw['Longitud (WGS84)']);
      final rotulo = raw['Rótulo']?.toString().trim() ?? '';
      final direccion = raw['Dirección']?.toString().trim() ?? '';
      final localidad = raw['Localidad']?.toString().trim() ?? '';
      if (lat == null || lng == null || rotulo.isEmpty) continue;

      final prices = <String, double?>{
        'BIE': _parsePrice(raw['Precio Bioetanol']),
        'BIO': _parsePrice(raw['Precio Biodiesel']),
        'G95E10': _parsePrice(raw['Precio Gasolina 95 E10']),
        'G95E5': _parsePrice(raw['Precio Gasolina 95 E5']),
        'G95E5Plus': _parsePrice(raw['Precio Gasolina 95 E5 Premium']),
        'G98E10': _parsePrice(raw['Precio Gasolina 98 E10']),
        'G98E5': _parsePrice(raw['Precio Gasolina 98 E5']),
        'GLP': _parsePrice(raw['Precio Gases licuados del petróleo']),
        'GNC': _parsePrice(raw['Precio Gas Natural Comprimido']),
        'GNL': _parsePrice(raw['Precio Gas Natural Licuado']),
        'GOA': _parsePrice(raw['Precio Gasoleo A']),
        'GOAPlus': _parsePrice(raw['Precio Gasoleo Premium']),
        'GOB': _parsePrice(raw['Precio Gasoleo B']),
      };

      final gasStation = GasStationModel(
        lat: lat,
        lng: lng,
        rotulo: rotulo,
        localizacion: '$direccion · $localidad',
        prices: prices,
      );

      if (selectedProductIds.isNotEmpty &&
          selectedProductIds
              .every((productId) => gasStation.priceFor(productId) == null)) {
        continue;
      }

      final distanceToRoute =
          GeoUtils.distanceToRouteKm(gasStation.location, routePoints);
      if (distanceToRoute > maxDistanceInKm) continue;

      parsedStations.add(gasStation);
    }

    parsedStations.sort((a, b) {
      final da = GeoUtils.distanceToRouteKm(a.location, routePoints);
      final db = GeoUtils.distanceToRouteKm(b.location, routePoints);

      return da.compareTo(db);
    });

    return parsedStations;
  }

  static double? _parsePrice(dynamic raw) {
    final text = raw?.toString();
    if (text == null || text.isEmpty) return null;
    final normalized = text.replaceAll(',', '.').replaceAll('€', '').trim();

    return double.tryParse(normalized);
  }

  static double? _parseDouble(dynamic raw) {
    final text = raw?.toString();
    if (text == null || text.isEmpty) return null;
    final normalized = text.replaceAll(',', '.').trim();

    return double.tryParse(normalized);
  }
}

class GoogleDirectionsService {
  final String apiKey;

  GoogleDirectionsService({required this.apiKey});

  Future<RouteInfo?> getRoute(String origin, String destination) async {
    if (origin.isEmpty || destination.isEmpty) return null;

    final uri = Uri.https(
      'maps.googleapis.com',
      '/maps/api/directions/json',
      {
        'origin': origin,
        'destination': destination,
        'key': apiKey,
        'units': 'metric',
      },
    );

    final response = await http.get(uri);
    if (response.statusCode != 200) return null;

    final body = jsonDecode(response.body) as Map<String, dynamic>;
    final routes = body['routes'] as List<dynamic>?;
    if (routes == null || routes.isEmpty) return null;

    final firstRoute = routes.first as Map<String, dynamic>;
    final overviewPolyline =
        firstRoute['overview_polyline'] as Map<String, dynamic>?;
    final encoded = overviewPolyline?['points'] as String?;
    if (encoded == null || encoded.isEmpty) return null;

    final legs = firstRoute['legs'] as List<dynamic>?;
    final firstLeg =
        legs?.isNotEmpty == true ? legs!.first as Map<String, dynamic> : null;
    final distanceText =
        (firstLeg?['distance'] as Map<String, dynamic>?)?['text'] as String? ??
            '';
    final durationText =
        (firstLeg?['duration'] as Map<String, dynamic>?)?['text'] as String? ??
            '';

    final points = GooglePolyline.decode(encoded);
    final bounds = _calculateBounds(points);

    return RouteInfo(
      encodedPolyline: encoded,
      points: points,
      bounds: bounds,
      distanceText: distanceText,
      durationText: durationText,
    );
  }

  LatLngBounds _calculateBounds(List<LatLng> points) {
    final lats = points.map((p) => p.latitude).toList();
    final lngs = points.map((p) => p.longitude).toList();

    return LatLngBounds(
      southwest: LatLng(lats.reduce(min), lngs.reduce(min)),
      northeast: LatLng(lats.reduce(max), lngs.reduce(max)),
    );
  }
}

class GooglePlacesService {
  final String apiKey;

  GooglePlacesService({required this.apiKey});

  Future<List<String>> findPlaces(String input) async {
    if (input.isEmpty) return [];

    final uri = Uri.https(
      'maps.googleapis.com',
      '/maps/api/place/autocomplete/json',
      {
        'input': input,
        'key': apiKey,
        'types': 'geocode',
        'language': 'es',
      },
    );

    final response = await http.get(uri, headers: headers);
    if (response.statusCode != 200) return [];

    final data = jsonDecode(response.body) as Map<String, dynamic>;
    final predictions = data['predictions'] as List<dynamic>?;
    if (predictions == null) return [];

    return predictions
        .map((item) => (item as Map<String, dynamic>)['description'] as String?)
        .whereType<String>()
        .toSet()
        .toList();
  }
}

class GooglePolyline {
  static List<LatLng> decode(String encoded) {
    final List<LatLng> coordinates = [];
    int index = 0;
    int lat = 0;
    int lng = 0;

    while (index < encoded.length) {
      final int shift = 0;
      int result = 0;
      int b;
      do {
        b = encoded.codeUnitAt(index++) - 63;
        result |= (b & 0x1F) << shift;
      } while (b >= 0x20);
      final int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
      lat += dlat;

      final int shift2 = 0;
      int result2 = 0;
      do {
        b = encoded.codeUnitAt(index++) - 63;
        result2 |= (b & 0x1F) << shift2;
      } while (b >= 0x20);
      final int dlng = ((result2 & 1) != 0 ? ~(result2 >> 1) : (result2 >> 1));
      lng += dlng;

      coordinates.add(LatLng(lat / 1e5, lng / 1e5));
    }

    return coordinates;
  }
}

class GeoUtils {
  static const double _earthRadiusKm = 6371.0;

  static double distanceKm(LatLng a, LatLng b) {
    final lat1 = _degreesToRadians(a.latitude);
    final lon1 = _degreesToRadians(a.longitude);
    final lat2 = _degreesToRadians(b.latitude);
    final lon2 = _degreesToRadians(b.longitude);
    final dlat = lat2 - lat1;
    final dlon = lon2 - lon1;
    final haversine =
        pow(sin(dlat / 2), 2) + cos(lat1) * cos(lat2) * pow(sin(dlon / 2), 2);
    final c = 2 * atan2(sqrt(haversine), sqrt(1 - haversine));
    return _earthRadiusKm * c;
  }

  static double distanceToRouteKm(LatLng point, List<LatLng> routePoints) {
    if (routePoints.isEmpty) {
      return double.infinity;
    }

    double minDistance = double.infinity;
    for (final routePoint in routePoints) {
      final distance = distanceKm(point, routePoint);
      if (distance < minDistance) {
        minDistance = distance;
      }
    }
    return minDistance;
  }

  static double _degreesToRadians(double degrees) => degrees * pi / 180.0;
}
