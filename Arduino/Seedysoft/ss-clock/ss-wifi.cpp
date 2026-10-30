#include <ESP8266WiFi.h>

#include "ss-wifi.h"

namespace Seedy {
  void setupESP8266WiFi() {
    const char* ssid = "wifiname";
    const char* pass = "wifipass";
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, pass);

    while (WiFi.status() != WL_CONNECTED) {
      Serial.print(".");
      delay(1000);
    }

    Serial.println("WiFi connected");
    Serial.printf("Board Hostname: %s\n", WiFi.getHostname());
    Serial.printf("    IP address: %s\n", WiFi.localIP().toString().c_str());
    Serial.printf("   MAC Address: %s\n", WiFi.macAddress().c_str());
  }
}
