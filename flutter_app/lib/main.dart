import 'package:flutter/material.dart';
import 'pages/travel_search_page.dart';

void main() {
  runApp(const SeedysoftTravelApp());
}

class SeedysoftTravelApp extends StatelessWidget {
  const SeedysoftTravelApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Seedysoft Travel Search',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.blueGrey),
        useMaterial3: true,
      ),
      home: const TravelSearchPage(),
    );
  }
}
