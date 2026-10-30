#include <SeedysoftESP8266WiFi.h>
#include <SeedysoftArduinoOTA.h>
#include <SeedysoftTM1637TinyDisplay.h>
#include <SeedysoftTimezoneNDP.h>
#include <SeedysoftPvpc.h>

const int LOOP_DELAY = 5000;  // 5 seconds

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);

  /* switch on led */
  digitalWrite(LED_BUILTIN, LOW);

  Serial.begin(115200);
  Serial.println("Booting");

  delay(LOOP_DELAY);

  setupESP8266WiFi();

  setupArduinoOTA();

  setupTimezoneNTPClient();

  setupTM1637TinyDisplay();

  setupSeedysoftPvpc();

  /* switch off led */
  digitalWrite(LED_BUILTIN, HIGH);
}

bool isTimeShow;

void loop() {
  ArduinoOTA.handle();

  if (isTimeShow) {
    showPrice(getPrice());
  } else {
    showTime(true);
  }

  // Alternate
  isTimeShow = !isTimeShow;

  delay(LOOP_DELAY);
}
