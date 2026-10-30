#include <NTPClient.h>
#include <WiFiUdp.h>
#include <Timezone.h>
#include <TimeLib.h>

#include "ss-time.h"

namespace Seedy {
  namespace {
    WiFiUDP ntpUDP;
    NTPClient timeClient(ntpUDP);

    // Central European Time (Bruselas, Copenhage, Madrid, París)
    TimeChangeRule CEST = { "CEST", Last, Sun, Mar, 2, 120 };
    TimeChangeRule CET = { "CET ", Last, Sun, Oct, 3, 60 };
    Timezone CE(CEST, CET);
  }

  void setupTimezoneNTPClient() {
    timeClient.begin();

    delay(1000);

    if (timeClient.update()) {
      Serial.println("Adjusting local clock");
      setTime(CE.toLocal(timeClient.getEpochTime()));
    }
    else {
      Serial.println("NTP Update not WORK!!");
    }
  }
}