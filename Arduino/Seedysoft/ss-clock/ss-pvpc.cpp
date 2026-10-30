#include <ESP8266HTTPClient.h>
#include <ESP8266WiFi.h>
#include <TimeLib.h>

#include "ss-pvpc.h"

namespace Seedy {
  namespace {
    const char* HourPriceUrl = "https://192.168.1.33:5364/get-pvpc";
    HourAndPrice hap;

    String httpGetRequest(const char* serverName) {
      WiFiClient client;
      HTTPClient http;

      if (!client.connect("192.168.1.33", 5364)) Serial.println("TCP failed");

      String payload = "";

      Serial.print("[HTTP] begin ");
      if (http.begin(client, serverName)) {
        Serial.println("OK");

        Serial.println("[HTTP] GET...");
        int httpCode = -1000;
        while (httpCode < 0) {
          httpCode = http.GET();
          Serial.printf("[HTTP] GET... code: %d\n", httpCode);

          if (httpCode > 0) {
            if (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
              payload = http.getString();
              Serial.println(payload);
            }
          } else {
            Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
            delay(1000);
          }
        }

        http.end();
      } else {
        Serial.println("Fail");
      }

      return payload;
    }
  }

  void setupSeedysoftPvpc() {
    hap.hour = 25;
    hap.price = " -- ";
  }

  String getPrice() {
    if (hap.hour == hour()) {
      // Serial.printf("Same hour: %d\n", hap.hour);
    } else {
      hap.hour = hour();

      // Serial.printf("Different hour: %d\n", hap.hour);

      if (WiFi.status() == WL_CONNECTED) {
        hap.price = httpGetRequest(HourPriceUrl);
      } else {
        hap.price = " -- ";

        Serial.printf("WiFi status is %u!!\n", WiFi.status());
      }
    }

    return hap.price;
  }
}
