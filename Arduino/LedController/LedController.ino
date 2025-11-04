#include <SeedysoftCredentials.h>
#include <ESP8266WiFi.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <FastLED.h>

const char* host = "esp8266-LED";

#define DATA_PIN D4  // change this to the pin connected on the ESP8266 to the WS2812 data pin
// #define LED_TYPE WS2812  // change this if the LED type is different than WS2812

WiFiServer server(80);

void setup() {
  Serial.begin(baud);
  Serial.println("Booting");

  // pinMode(LED_BUILTIN, OUTPUT);
  pinMode(DATA_PIN, OUTPUT);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pass);

  while (WiFi.waitForConnectResult() != WL_CONNECTED) {
    Serial.println("Connection Failed! Rebooting...");
    delay(5000);
    ESP.restart();
  }

  // Port defaults to 8266
  // ArduinoOTA.setPort(8266);

  // Hostname defaults to esp8266-[ChipID]
  // ArduinoOTA.setHostname("myesp8266");
  ArduinoOTA.setHostname(host);

  // No authentication by default
  // ArduinoOTA.setPassword("admin");

  // Password can be set with it's md5 value as well
  // MD5(admin) = 21232f297a57a5a743894a0e4a801fc3
  // ArduinoOTA.setPasswordHash("21232f297a57a5a743894a0e4a801fc3");

  ArduinoOTA.onStart([]() {
    String type;
    if (ArduinoOTA.getCommand() == U_FLASH) type = "sketch";
    else type = "filesystem";

    // NOTE: if updating FS this would be the place to unmount FS using FS.end()
    Serial.println("Start updating " + type);
  });
  ArduinoOTA.onEnd([]() {
    Serial.println("\nEnd");
  });
  ArduinoOTA.onProgress([](unsigned int progress, unsigned int total) {
    Serial.printf("Progress: %u%%\r", (progress / (total / 100)));
  });
  ArduinoOTA.onError([](ota_error_t error) {
    Serial.printf("Error[%u]: ", error);
    if (error == OTA_AUTH_ERROR) Serial.println("Auth Failed");
    else if (error == OTA_BEGIN_ERROR) Serial.println("Begin Failed");
    else if (error == OTA_CONNECT_ERROR) Serial.println("Connect Failed");
    else if (error == OTA_RECEIVE_ERROR) Serial.println("Receive Failed");
    else if (error == OTA_END_ERROR) Serial.println("End Failed");
  });
  ArduinoOTA.begin();

  Serial.println("Ready");
  Serial.print("IP  address: ");
  Serial.println(WiFi.localIP());
  Serial.print("MAC address: ");
  Serial.println(WiFi.macAddress());
}

void loop() {
  ArduinoOTA.handle();

  // int ledStatus = digitalRead(LED_BUILTIN);
  // Serial.print("LED builtin is ");
  // Serial.println(ledStatus);
  // delay(1000);

  digitalWrite(DATA_PIN, HIGH);
  Serial.println("turn the LED on (HIGH is the voltage level)");
  delay(3000);
  digitalWrite(DATA_PIN, LOW);
  Serial.println("turn the LED off by making the voltage LOW");
  delay(3000);

  // digitalWrite(LED_BUILTIN, ledStatus == HIGH ? LOW : HIGH);
  // Serial.print("turn the builtin LED ");
  // Serial.println(ledStatus == 1 ? "HIGH" : "LOW");
  // delay(2000);
}
