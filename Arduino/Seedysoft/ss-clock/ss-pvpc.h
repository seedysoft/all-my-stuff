#pragma once

#include <Arduino.h>

namespace Seedy {
  struct HourAndPrice {
    int hour;
    String price;
  };

  void setupSeedysoftPvpc();
  String getPrice();
}
