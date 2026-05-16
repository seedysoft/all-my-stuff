import 'package:flutter/material.dart';
import 'package:flutter_typeahead/flutter_typeahead.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../models.dart';
import '../services.dart';

const String googleApiKey = '<<YOUR_GOOGLE_API_KEY>>';

class TravelSearchPage extends StatefulWidget {
  const TravelSearchPage({super.key});

  @override
  State<TravelSearchPage> createState() => _TravelSearchPageState();
}

class _TravelSearchPageState extends State<TravelSearchPage> {
  final _formKey = GlobalKey<FormState>();
  final _originController = TextEditingController();
  final _destinationController = TextEditingController();
  final _queryModel = TravelQueryModel(
    origin: 'Calle Juan Ramon Jimenez, 8, Burgos, Spain',
    destination: 'Manciles, Spain',
    maxDistanceKm: 10,
    selectedProductIds: defaultSelectedProductIds.toSet(),
  );

  late final GooglePlacesService _placesService;
  late final GoogleDirectionsService _directionsService;
  late final GasStationPricesService _gasStationPricesService;

  GoogleMapController? _mapController;
  RouteInfo? _routeInfo;
  Set<Marker> _markers = {};
  Set<Polyline> _polylines = {};
  List<GasStationModel> _gasStations = [];
  bool _isLoading = false;
  GasStationModel? _selectedStation;

  static const Set<String> _gasolineProductIds = {
    'G95E10',
    'G95E5',
    'G95E5Plus',
    'G98E10',
    'G98E5',
  };

  static const Set<String> _dieselProductIds = {
    'GOA',
    'GOAPlus',
    'GOB',
  };

  @override
  void initState() {
    super.initState();
    _placesService = GooglePlacesService(apiKey: googleApiKey);
    _directionsService = GoogleDirectionsService(apiKey: googleApiKey);
    _gasStationPricesService = GasStationPricesService();
  }

  @override
  void dispose() {
    _originController.dispose();
    _destinationController.dispose();
    super.dispose();
  }

  Future<void> _searchRoute() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isLoading = true;
      _gasStations = [];
      _selectedStation = null;
      _routeInfo = null;
    });

    try {
      final origin = _originController.text.trim();
      final destination = _destinationController.text.trim();
      final routeInfo = await _directionsService.getRoute(origin, destination);
      if (routeInfo == null) {
        _showMessage('No route could be found. Check origin and destination.');
        return;
      }

      _routeInfo = routeInfo;
      _updateRouteOnMap(routeInfo);

      final stations = await _gasStationPricesService.getNearGasStations(
        routeInfo.encodedPolyline,
        _queryModel.maxDistanceKm,
        _queryModel.selectedProductIds,
      );

      setState(() {
        _gasStations = stations;
      });
    } catch (e) {
      _showMessage('Search failed: $e');
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  void _updateRouteOnMap(RouteInfo routeInfo) {
    final polyline = Polyline(
      polylineId: const PolylineId('route'),
      points: routeInfo.points,
      color: Colors.blueAccent,
      width: 5,
    );

    final allMarkers = <Marker>{
      Marker(
        markerId: const MarkerId('origin'),
        position: routeInfo.points.first,
        infoWindow: const InfoWindow(title: 'Origin'),
      ),
      Marker(
        markerId: const MarkerId('destination'),
        position: routeInfo.points.last,
        infoWindow: const InfoWindow(title: 'Destination'),
      ),
    };

    setState(() {
      _polylines = {polyline};
      _markers = allMarkers;
    });

    _mapController?.animateCamera(CameraUpdate.newLatLngBounds(routeInfo.bounds, 50));
  }

  void _updateSelectedProducts(String productId, bool isSelected) {
    setState(() {
      if (isSelected) {
        _queryModel.selectedProductIds.add(productId);
      } else {
        _queryModel.selectedProductIds.remove(productId);
      }
    });
  }

  void _setSelectedProductIds(Set<String> ids) {
    setState(() {
      _queryModel.selectedProductIds = ids;
    });
  }

  void _showMessage(String message) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(message)));
  }

  void _setSelectedStation(GasStationModel? station) {
    setState(() {
      _markers.removeWhere((marker) => marker.markerId.value.startsWith('station-'));
      _selectedStation = station;
      if (station != null) {
        _markers.add(
          Marker(
            markerId: MarkerId('station-${station.lat}-${station.lng}'),
            position: station.location,
            icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueGreen),
            infoWindow: InfoWindow(title: station.rotulo, snippet: station.localizacion),
          ),
        );
      }
    });

    if (station != null) {
      _mapController?.animateCamera(CameraUpdate.newLatLngZoom(station.location, 13));
    }
  }

  Widget _buildProductSelection() {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: allProducts.map((product) {
        return FilterChip(
          label: Text(product.label),
          selected: _queryModel.selectedProductIds.contains(product.id),
          onSelected: (selected) => _updateSelectedProducts(product.id, selected),
          selectedColor: Colors.blue.shade100,
        );
      }).toList(),
    );
  }

  Widget _buildProductSelectionActions() {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: [
        ElevatedButton.icon(
          onPressed: () => _setSelectedProductIds(allProducts.map((product) => product.id).toSet()),
          icon: const Icon(Icons.select_all),
          label: const Text('All'),
        ),
        OutlinedButton.icon(
          onPressed: () => _setSelectedProductIds(_gasolineProductIds),
          icon: const Icon(Icons.ev_station),
          label: const Text('Gasoline'),
        ),
        OutlinedButton.icon(
          onPressed: () => _setSelectedProductIds(_dieselProductIds),
          icon: const Icon(Icons.local_shipping),
          label: const Text('Diesel'),
        ),
        TextButton(
          onPressed: () => _setSelectedProductIds({}),
          child: const Text('None'),
        ),
      ],
    );
  }

  Widget _buildRouteSummary() {
    final routeInfo = _routeInfo!;
    final selectedProducts = _queryModel.selectedProductIds.isEmpty
        ? allProducts
        : allProducts.where((product) => _queryModel.selectedProductIds.contains(product.id));

    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Route details', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                Chip(
                  label: Text('${_gasStations.length} stations'),
                  backgroundColor: Colors.blue.shade50,
                ),
              ],
            ),
            const SizedBox(height: 12),
            Wrap(
              runSpacing: 6,
              spacing: 10,
              children: [
                Chip(label: Text('Distance: ${routeInfo.distanceText}')),
                Chip(label: Text('Duration: ${routeInfo.durationText}')),
                Chip(label: Text('Products: ${selectedProducts.length}')),
                Chip(label: Text('Max detour: ${_queryModel.maxDistanceKm} km')),
              ],
            ),
            const SizedBox(height: 12),
            const Text('Selected products', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
            const SizedBox(height: 6),
            Wrap(
              spacing: 6,
              runSpacing: 6,
              children: selectedProducts
                  .map(
                    (product) => Chip(
                      label: Text(product.label),
                      visualDensity: VisualDensity.compact,
                      backgroundColor: Colors.blue.shade50,
                    ),
                  )
                  .toList(),
            ),
            const SizedBox(height: 12),
            Text('Origin: ${_originController.text}', style: const TextStyle(fontSize: 13, color: Colors.black87)),
            Text('Destination: ${_destinationController.text}', style: const TextStyle(fontSize: 13, color: Colors.black87)),
          ],
        ),
      ),
    );
  }

  Widget _buildSelectedStationDetails() {
    final station = _selectedStation!;
    final selectedProducts = _queryModel.selectedProductIds.isEmpty
        ? allProducts
        : allProducts.where((product) => _queryModel.selectedProductIds.contains(product.id));

    final distanceToRoute = _routeInfo == null
        ? null
        : GeoUtils.distanceToRouteKm(station.location, _routeInfo!.points);

    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(station.rotulo, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                      const SizedBox(height: 4),
                      Text(station.localizacion, style: const TextStyle(fontSize: 12, color: Colors.black54)),
                    ],
                  ),
                ),
                Text('Selected', style: TextStyle(color: Colors.green.shade700, fontWeight: FontWeight.bold)),
              ],
            ),
            const SizedBox(height: 12),
            if (distanceToRoute != null)
              Text('Distance to route: ${distanceToRoute.toStringAsFixed(1)} km', style: const TextStyle(fontSize: 12)),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: selectedProducts
                  .map((product) => Chip(
                        label: Text(product.label),
                        backgroundColor: Colors.blueGrey.shade50,
                      ))
                  .toList(),
            ),
            const SizedBox(height: 12),
            const Text('Prices', style: TextStyle(fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Column(
              children: selectedProducts.map((product) {
                return Padding(
                  padding: const EdgeInsets.symmetric(vertical: 2.0),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(productName(product.id)),
                      Text(station.priceForLabel(product.id)),
                    ],
                  ),
                );
              }).toList(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildGasStationsTableCard() {
    final selectedProducts = _queryModel.selectedProductIds.toList();
    final showProducts = selectedProducts.isNotEmpty ? selectedProducts : [];

    final dataColumns = <DataColumn>[
      const DataColumn(label: Text('Station')),
      const DataColumn(label: Text('Location')),
      const DataColumn(label: Text('Route\nkm'), numeric: true),
      ...showProducts.map((productId) {
        return DataColumn(
          label: Text(productId, textAlign: TextAlign.center),
          numeric: true,
        );
      }),
    ];

    final dataRows = _gasStations.map((station) {
      final isSelected = _selectedStation == station;
      final distanceToRoute = _routeInfo == null
          ? null
          : GeoUtils.distanceToRouteKm(station.location, _routeInfo!.points);

      return DataRow(
        selected: isSelected,
        onSelectChanged: (_) {
          if (isSelected) {
            _setSelectedStation(null);
          } else {
            _setSelectedStation(station);
          }
        },
        cells: [
          DataCell(Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(station.rotulo, style: const TextStyle(fontWeight: FontWeight.bold)),
              Text(station.localizacion, style: const TextStyle(fontSize: 12, color: Colors.black54)),
            ],
          )),
          DataCell(Text(station.localizacion, maxLines: 1, overflow: TextOverflow.ellipsis)),
          DataCell(Text(distanceToRoute == null ? '-' : '${distanceToRoute.toStringAsFixed(1)} km')),
          ...showProducts.map((productId) => DataCell(Text(station.priceForLabel(productId), textAlign: TextAlign.right))),
        ],
      );
    }).toList();

    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Gas station price table', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                Text('${_gasStations.length} items', style: const TextStyle(color: Colors.black54)),
              ],
            ),
            const SizedBox(height: 12),
            if (selectedProducts.isEmpty)
              const Text('Select products above to show prices in the table.', style: TextStyle(color: Colors.black54))
            else
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: DataTable(
                  columns: dataColumns,
                  rows: dataRows,
                  headingRowColor: WidgetStateProperty.resolveWith((states) => Colors.blueGrey.shade50),
                  dataRowColor: WidgetStateProperty.resolveWith((states) {
                    if (states.contains(WidgetState.selected)) {
                      return Colors.blue.shade50;
                    }
                    return null;
                  }),
                  columnSpacing: 24,
                  showCheckboxColumn: false,
                ),
              ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Travel Search')),
      body: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          children: [
            Form(
              key: _formKey,
              child: Column(
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: TypeAheadFormField<String>(
                          textFieldConfiguration: TextFieldConfiguration(
                            controller: _originController,
                            decoration: const InputDecoration(
                              labelText: 'Origin',
                              border: OutlineInputBorder(),
                            ),
                          ),
                          suggestionsCallback: (pattern) => _placesService.findPlaces(pattern),
                          itemBuilder: (context, suggestion) => ListTile(title: Text(suggestion)),
                          onSuggestionSelected: (suggestion) => _originController.text = suggestion,
                          validator: (value) => value == null || value.isEmpty ? 'Origin is required' : null,
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: TypeAheadFormField<String>(
                          textFieldConfiguration: TextFieldConfiguration(
                            controller: _destinationController,
                            decoration: const InputDecoration(
                              labelText: 'Destination',
                              border: OutlineInputBorder(),
                            ),
                          ),
                          suggestionsCallback: (pattern) => _placesService.findPlaces(pattern),
                          itemBuilder: (context, suggestion) => ListTile(title: Text(suggestion)),
                          onSuggestionSelected: (suggestion) => _destinationController.text = suggestion,
                          validator: (value) => value == null || value.isEmpty ? 'Destination is required' : null,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text('Max distance (${_queryModel.maxDistanceKm} km)'),
                            Slider(
                              value: _queryModel.maxDistanceKm.toDouble(),
                              min: 1,
                              max: 50,
                              divisions: 49,
                              label: '${_queryModel.maxDistanceKm} km',
                              onChanged: (value) {
                                setState(() {
                                  _queryModel.maxDistanceKm = value.round();
                                });
                              },
                            ),
                          ],
                        ),
                      ),
                      ElevatedButton.icon(
                        onPressed: _isLoading ? null : _searchRoute,
                        icon: const Icon(Icons.search),
                        label: const Text('Search'),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            Expanded(
              flex: 2,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('Fuel selection', style: TextStyle(fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  _buildProductSelectionActions(),
                  const SizedBox(height: 8),
                  _buildProductSelection(),
                  const SizedBox(height: 16),
                  if (_routeInfo != null) ...[
                    _buildRouteSummary(),
                    const SizedBox(height: 16),
                  ],
                  Expanded(
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: GoogleMap(
                        initialCameraPosition: const CameraPosition(
                          target: LatLng(42.3432, -3.6969),
                          zoom: 6,
                        ),
                        onMapCreated: (controller) => _mapController = controller,
                        markers: _markers,
                        polylines: _polylines,
                        myLocationEnabled: false,
                        compassEnabled: true,
                      ),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            Expanded(
              flex: 1,
              child: _isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : _gasStations.isEmpty
                      ? const Center(child: Text('No gas stations to show. Run a search to load results.'))
                      : Column(
                          children: [
                            if (_selectedStation != null) ...[
                              _buildSelectedStationDetails(),
                              const SizedBox(height: 12),
                            ],
                            Expanded(
                              child: _buildGasStationsTableCard(),
                            ),
                          ],
                        ),
            ),
          ],
        ),
      ),
    );
  }
}
