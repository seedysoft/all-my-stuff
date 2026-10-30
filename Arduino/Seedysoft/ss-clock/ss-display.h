#pragma once

#include <Arduino.h>

namespace Seedy {
  void setupTM1637TinyDisplay();
  void showPrice(String price);
  void showTime(bool isColonShow);
}
