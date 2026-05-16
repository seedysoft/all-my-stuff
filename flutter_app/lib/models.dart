import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

class ProductInfo {
  final String id;
  final String label;
  final IconData icon;

  const ProductInfo({
    required this.id,
    required this.label,
    required this.icon,
  });
}

const Map<String, String> headers = {
  'Accept': 'application/json',
  'Access-Control-Allow-Origin': '*',
  'mode': 'cors',
  'credentials': 'include'
};

const List<ProductInfo> allProducts = [
  ProductInfo(id: 'BIE', label: 'BIE', icon: Icons.local_gas_station),
  ProductInfo(id: 'BIO', label: 'BIO', icon: Icons.local_gas_station),
  ProductInfo(id: 'G95E10', label: 'G95E10', icon: Icons.local_gas_station),
  ProductInfo(id: 'G95E5', label: 'G95E5', icon: Icons.local_gas_station),
  ProductInfo(id: 'G95E5Plus', label: 'G95E5+', icon: Icons.local_gas_station),
  ProductInfo(id: 'G98E10', label: 'G98E10', icon: Icons.local_gas_station),
  ProductInfo(id: 'G98E5', label: 'G98E5', icon: Icons.local_gas_station),
  ProductInfo(id: 'GLP', label: 'GLP', icon: Icons.local_gas_station),
  ProductInfo(id: 'GNC', label: 'GNC', icon: Icons.local_gas_station),
  ProductInfo(id: 'GNL', label: 'GNL', icon: Icons.local_gas_station),
  ProductInfo(id: 'GOA', label: 'GOA', icon: Icons.local_gas_station),
  ProductInfo(id: 'GOAPlus', label: 'GOA+', icon: Icons.local_gas_station),
  ProductInfo(id: 'GOB', label: 'GOB', icon: Icons.local_gas_station),
];

const List<String> defaultSelectedProductIds = [
  'G95E10',
  'G95E5',
  'G95E5Plus',
  'G98E10',
  'G98E5',
];

String productPriceKey(String productId) {
  return switch (productId) {
    'BIE' => 'Precio Bioetanol',
    'BIO' => 'Precio Biodiesel',
    'G95E10' => 'Precio Gasolina 95 E10',
    'G95E5' => 'Precio Gasolina 95 E5',
    'G95E5Plus' => 'Precio Gasolina 95 E5 Premium',
    'G98E10' => 'Precio Gasolina 98 E10',
    'G98E5' => 'Precio Gasolina 98 E5',
    'GLP' => 'Precio Gases licuados del petróleo',
    'GNC' => 'Precio Gas Natural Comprimido',
    'GNL' => 'Precio Gas Natural Licuado',
    'GOA' => 'Precio Gasoleo A',
    'GOAPlus' => 'Precio Gasoleo Premium',
    'GOB' => 'Precio Gasoleo B',
    _ => productId,
  };
}

String productName(String productId) {
  return switch (productId) {
    'BIE' => 'Bioetanol',
    'BIO' => 'Biodiésel',
    'G95E10' => 'Gasolina 95 E10',
    'G95E5' => 'Gasolina 95 E5',
    'G95E5Plus' => 'Gasolina 95 E5 Premium',
    'G98E10' => 'Gasolina 98 E10',
    'G98E5' => 'Gasolina 98 E5',
    'GLP' => 'GLP',
    'GNC' => 'GNC',
    'GNL' => 'GNL',
    'GOA' => 'Gasóleo A',
    'GOAPlus' => 'Gasóleo Premium',
    'GOB' => 'Gasóleo B',
    _ => productId,
  };
}

class TravelQueryModel {
  String origin;
  String destination;
  int maxDistanceKm;
  Set<String> selectedProductIds;

  TravelQueryModel({
    this.origin = '',
    this.destination = '',
    this.maxDistanceKm = 10,
    Set<String>? selectedProductIds,
  }) : selectedProductIds =
            selectedProductIds ?? defaultSelectedProductIds.toSet();
}

class GasStationModel {
  final double lat;
  final double lng;
  final String rotulo;
  final String localizacion;
  final Map<String, double?> prices;

  GasStationModel({
    required this.lat,
    required this.lng,
    required this.rotulo,
    required this.localizacion,
    required this.prices,
  });

  LatLng get location => LatLng(lat, lng);

  double? priceFor(String productId) => prices[productId];

  String priceForLabel(String productId) {
    final value = priceFor(productId);
    return value == null ? 'N/A' : '${value.toStringAsFixed(3)} €';
  }
}

class RouteInfo {
  final String encodedPolyline;
  final List<LatLng> points;
  final LatLngBounds bounds;
  final String distanceText;
  final String durationText;

  RouteInfo({
    required this.encodedPolyline,
    required this.points,
    required this.bounds,
    required this.distanceText,
    required this.durationText,
  });
}
