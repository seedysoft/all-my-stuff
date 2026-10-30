#include "ss-arduino-ota.h"
#include "ss-display.h"
#include "ss-pvpc.h"
#include "ss-time.h"
#include "ss-wifi.h"

#include <ArduinoOTA.h>

namespace Seedy {
  const int LOOP_DELAY = 5000;
  bool isTimeShow;
}

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);

  /* switch on led */
  digitalWrite(LED_BUILTIN, LOW);

  Serial.begin(115200);
  Serial.println("Booting");

  delay(Seedy::LOOP_DELAY);

  Seedy::setupESP8266WiFi();

  Seedy::setupArduinoOTA();

  Seedy::setupTimezoneNTPClient();

  Seedy::setupTM1637TinyDisplay();

  Seedy::setupSeedysoftPvpc();

  /* switch off led */
  digitalWrite(LED_BUILTIN, HIGH);
}

void loop() {
  ArduinoOTA.handle();

  if (Seedy::isTimeShow) {
    Seedy::showPrice(Seedy::getPrice());
  } else {
    Seedy::showTime(true);
  }

  // Alternate
  Seedy::isTimeShow = !Seedy::isTimeShow;

  delay(Seedy::LOOP_DELAY);
}
