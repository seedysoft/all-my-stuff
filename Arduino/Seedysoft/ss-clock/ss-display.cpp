#include <Arduino.h>
#include <TM1637TinyDisplay.h>
#include <TimeLib.h>

#include "ss-display.h"

namespace Seedy {
  namespace {
    TM1637TinyDisplay display(D2, D3);
    char buffer[5];
  }

  void setupTM1637TinyDisplay() {
    display.begin();
    display.setBrightness(BRIGHT_LOW);
  }

  void showPrice(String price) {
    snprintf(buffer, sizeof(buffer), "%s", price.c_str());

    Serial.println(buffer);

    display.showString(buffer, MAXDIGITS, 0, 0);
  }

  void showTime(bool isColonShow) {
    snprintf(buffer, sizeof(buffer), "%02d%02d", hour(), minute());

    Serial.println(buffer);

    display.showString(buffer, MAXDIGITS, 0, (isColonShow ? 0b01000000 : 0));
  }
}
