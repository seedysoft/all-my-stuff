(function () {
  let myMapObj = {
    gMap: {},
    wayPoints: [],
    paths: [],
    routeLabels: [],
    isAddress: {
      "origin": true,
      "destination": true
    },
    init: function () {
      let mapContainer = document.getElementById("map");

      let mapOptions = {
        center: { lat: -34.397, lng: 150.644 },
        zoom: 2,
        mapId: 'fbbfebe213a16ef5'
      };

      this.gMap = new google.maps.Map(mapContainer, mapOptions);
      this.mapEvents();
      document.getElementById("btn-submit").addEventListener('click', () => {
        myMapObj.sendRequest();
      });
    },
    mapEvents: function () {
      function clickGetLatLng() {
        let infowindowContainer = document.getElementById("infowindow-alert");
        let infoWindow = new google.maps.InfoWindow();
        let infoWindowAlert = document.getElementById("infowindow-alert");

        myMapObj.gMap.addListener('click', (mapsMouseEvent) => {
          let location = mapsMouseEvent.latLng.lat() + "," + mapsMouseEvent.latLng.lng();
          infoWindowAlert.style.visibility = "visible";
          infoWindow.close();
          infoWindow.setContent(infowindowContainer);
          infoWindow.setPosition({ lat: mapsMouseEvent.latLng.lat(), lng: mapsMouseEvent.latLng.lng() });
          navigator.clipboard.writeText(location).then(function () {
            infoWindow.open(myMapObj.gMap);
            setTimeout(function () { infoWindow.close() }, 2000);
          }, function () {

          });
        });
      }

      // Toggle between address input ad latlng/placeID input
      function inputToggle() {
        let inputToggleCheckbox = document.querySelectorAll(".input-toggle input");
        let autocomplete = [];
        let autocompleteLsr = [];

        function autoComplete(elem, i, enable) {
          if (enable) {
            autocomplete[i] = new google.maps.places.Autocomplete(elem);
            autocompleteLsr[i] = google.maps.event.addListener(autocomplete[i], "place_changed", () => {
              let place = autocomplete[i].getPlace();
              if (Object.keys(place).length > 0) {
                if (!place.geometry || !place.geometry.location) {
                  window.alert("No details available for input: '" + place.name + "'");
                  return;
                }
              }
            });
          } else {
            google.maps.event.clearInstanceListeners(elem);
          }

        }

        inputToggleCheckbox.forEach((item, index) => {
          let inputField = document.getElementById(item.dataset.type);
          autoComplete(inputField, index, true); // Address as waypoint and Autocomplete are enabled by default
          item.addEventListener('change', () => {
            if (item.checked) {
              inputField.placeholder = 'Enter lat,lng OR Place ID';
              inputField.value = '';
              myMapObj.isAddress[item.dataset.type] = false;
              autoComplete(inputField, index, false);
            } else {
              inputField.placeholder = 'Enter an address';
              inputField.value = '';
              myMapObj.isAddress[item.dataset.type] = true;
              autoComplete(inputField, index, true);
            }
          });
        })
      }

      // Toggle options for different travel mode
      function travelModeToggle() {
        let travelMode = document.getElementById("travel_mode");
        let travelPreference = document.getElementById("traffic");
        let trafficAwarePolyline = document.getElementById("traffic_aware_polyline");
        let pref = travelPreference.value;
        let ecoRoutes = document.getElementById("eco_routes");
        let emissionType = document.getElementById("emissiontype");

        travelMode.addEventListener('change', function () {
          let mode = this.value;
          pref = travelPreference.value;

          // Toggle the Traffic Preference selection and Traffic Aware Polyline selection for WALK mode and BICYCLE mode
          if (mode == "walk" || mode == "bicycle" || mode == "transit") {
            travelPreference.disabled = true;
            travelPreference.value = null;
            trafficAwarePolyline.checked = false;
            trafficAwarePolyline.disabled = true;
          } else {
            travelPreference.disabled = false;
            pref = travelPreference.value;

            if (!pref) {
              travelPreference.value = "traffic_unaware";
              pref = travelPreference.value;
            }
            if (pref == "traffic_unaware") {
              trafficAwarePolyline.checked = false;
              trafficAwarePolyline.disabled = true;
            } else {
              trafficAwarePolyline.disabled = false;
            }
          }
          // Toggle transition options for Transit mode
          if (mode == "transit") {
            document.getElementById("transit_options").style.display = "block";
          } else {
            document.getElementById("transit_options").style.display = "none";
          }
        });

        travelPreference.addEventListener('change', function () {
          pref = this.value;
          if (pref == "traffic_unaware") {
            trafficAwarePolyline.checked = false;
            trafficAwarePolyline.disabled = true;
          } else {
            trafficAwarePolyline.disabled = false;
          }
        });

        ecoRoutes.addEventListener("change", function () {
          if (this.checked) {
            emissionType.disabled = false;
          } else {
            emissionType.disabled = true;
          }
        });
      }

      function closeAlertBox() {
        let alertBox = document.getElementById("alert");
        let closeBtn = alertBox.querySelector(".close");
        closeBtn.addEventListener('click', () => {
          if (alertBox.style.display == "block") {
            alertBox.style.display = "none";
          }
        });
      }

      // Convert local time to UTC time
      function utcConversion() {
        let departureTime = document.getElementById('departuretime');
        let utcOutput = document.getElementById('utcoutput');
        departureTime.addEventListener('change', (e) => {
          utcOutput.innerHTML = "UTC time: " + new Date(e.target.value).toUTCString();
        });
      }
      clickGetLatLng();
      travelModeToggle();
      closeAlertBox();
      inputToggle();
      utcConversion();
    },
    sendRequest: function () {
      let reqBody = {};
      let originLatLng,
        destinationLatLng,
        origin = document.getElementById("origin").value.trim(),
        destination = document.getElementById("destination").value.trim(),
        heading_org = document.getElementById("heading_org").value.trim(),
        heading_des = document.getElementById("heading_des").value.trim(),
        routing_preference = document.getElementById("traffic").value,
        traffic_aware_polyline = document.getElementById("traffic_aware_polyline").checked,
        eco_routes = document.getElementById("eco_routes").checked,
        travel_mode = document.getElementById("travel_mode").value,
        departure_time = document.getElementById("departuretime").value;
      let requestedReferenceRoutes = [],
        extraComputations = [],
        allowedTravelModes = [];
      /******  Start of Request *****/
      reqBody = {
        "origin": {
          "vehicleStopover": document.getElementById("origin_stopover").checked,
          "sideOfRoad": document.getElementById("side_of_road_org").checked
        },
        "destination": {
          "vehicleStopover": document.getElementById("destination_stopover").checked,
          "sideOfRoad": document.getElementById("side_of_road_des").checked
        },
        "travelMode": travel_mode,
        "routingPreference": routing_preference == '' ? "ROUTING_PREFERENCE_UNSPECIFIED" : routing_preference,
        "polylineQuality": document.getElementById("polyline_quality").value,
        "computeAlternativeRoutes": document.getElementById("alternative_routes").checked,
        "routeModifiers": {
          "avoidTolls": document.getElementById("avoid_tolls").checked,
          "avoidHighways": document.getElementById("avoid_highways").checked,
          "avoidFerries": document.getElementById("avoid_ferries").checked,
          "avoidIndoor": document.getElementById("avoid_indoor").checked
        }
      };

      if (origin !== '') {
        if (myMapObj.isAddress.origin) {
          reqBody.origin.address = origin;
        } else {
          if (origin.indexOf(',') >= 0) {
            origin_lat_lng = origin.split(",");
            reqBody.origin.location = {
              "latLng": {
                "latitude": origin_lat_lng[0].trim(),
                "longitude": origin_lat_lng[1].trim()
              }
            };
          }
          else {
            reqBody.origin.placeId = origin;
          }
        }
      }
      else {
        myMapObj.setErrorMsg("Origin must be set in a right format.");
        return false;
      }
      if (destination !== '') {
        if (myMapObj.isAddress.destination) {
          reqBody.destination.address = destination;
        } else {
          if (destination.indexOf(',') >= 0) {
            destination_lat_lng = destination.split(",");
            reqBody.destination.location = {
              "latLng": {
                "latitude": destination_lat_lng[0].trim(),
                "longitude": destination_lat_lng[1].trim()
              }
            };
          }
          else {
            reqBody.destination.placeId = destination;
          }
        }
      }
      else {
        myMapObj.setErrorMsg("Destination must be set in a right format.");
        return false;
      }
      if (heading_org && reqBody.origin.location && travel_mode != "transit") {
        reqBody.origin.location.heading = heading_org;
      }
      if (heading_des && reqBody.destination.location && travel_mode != "transit") {
        reqBody.destination.location.heading = heading_des;
      }

      if (traffic_aware_polyline) {
        extraComputations.push("TRAFFIC_ON_POLYLINE");
        reqBody.extraComputations = extraComputations;
      }

      if (eco_routes) {
        requestedReferenceRoutes.push("FUEL_EFFICIENT");
        extraComputations.push("FUEL_CONSUMPTION");
        reqBody.requestedReferenceRoutes = requestedReferenceRoutes;
        reqBody.extraComputations = extraComputations;
        reqBody.routeModifiers.vehicleInfo = {
          "emissionType": document.getElementById("emissiontype").value
        };
      }

      if (travel_mode == "transit") {
        let selectedElem = document.querySelectorAll('ul#transitModes li input[type="checkbox"]:checked');
        reqBody.transitPreferences = {
          "allowedTravelModes": Array.from(selectedElem).map(x => x.value),
          "routingPreference": document.getElementById("transitPreference").value
        }
      }

      if (travel_mode != "transit") {
        if (routing_preference == '') {
          reqBody.routingPreference = "ROUTING_PREFERENCE_UNSPECIFIED";
        } else {
          reqBody.routingPreference = routing_preference;
        }
      }

      if (departure_time != '') {
        let date = new Date(departure_time);
        let utc_date = `${date.getUTCFullYear()}-${date.getUTCMonth() + 1}-${date.getUTCDate()}T${date.getUTCHours()}:${date.getUTCMinutes()}:00Z`;
        reqBody.departureTime = utc_date;
      }

      fetch("https://routes.googleapis.com/directions/v2:computeRoutes", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-Goog-Api-Key": "AIzaSyAOWd855Jru-vGD_bVJqc6Qr-n8VpX0XsA",
          "X-Goog-FieldMask": "*"
        },
        body: JSON.stringify(reqBody)
      }).then((response) => {
        return response.json();
      }).then((data) => {
        if ('error' in data) {
          myMapObj.setErrorMsg(data.error);
        } else if (!data.hasOwnProperty("routes")) {
          myMapObj.setErrorMsg("No routes found. It's likely the waypoints location have problems or the travel mode is not supported in this location. ");
        } else {
          myMapObj.setRoute(data);
        }
      }).catch((error) => {
        console.log(error)
      });

      /****** End of Request ******/

    },
    setErrorMsg: function (err) {
      let alert = document.getElementById("alert");
      if (typeof err === 'string') {
        alert.querySelector("p").innerHTML = err;
      } else if (typeof err === 'object') {
        let msg = err.hasOwnProperty("code") ? err.code : '';
        msg += err.hasOwnProperty("message") ? " " + err.message : '';
        alert.querySelector("p").innerHTML = msg;
      }
      alert.style.display = 'block';
    },
    setRoute: function (data) {
      this.clearUI(myMapObj.wayPoints, 'marker');
      this.clearUI(myMapObj.paths, 'routes');
      this.clearUI(myMapObj.routeLabels, 'advMarker');
      let colors = {
        "NORMAL": "#4285f4",
        "SLOW": "#fbbc04",
        "TRAFFIC_JAM": "#ea4335",
        "ALTERNATIVE": "#999999"
      };
      let routes = data.routes; //routes
      let speedIntervals = []; //traffic aware polyline indexes
      let decodedPaths = [];

      function addRouteLabel(location, route) {
        let routeTag = document.createElement('div');
        let content = '';
        routeTag.className = "route-tag";

        if (route.hasOwnProperty("routeLabels")) {
          content += "<p>"
          route.routeLabels.forEach((label, index) => {
            if (label.includes("FUEL_EFFICIENT")) {
              routeTag.classList.add("eco");
            }
            if (label.includes("DEFAULT_ROUTE_ALTERNATE")) {
              routeTag.classList.add("alternate");
            }
            content += label + (index == route.routeLabels.length - 1 ? "" : "<br />");
          });
          content += "</p>";
        }
        content += '<div class="details">';
        if (route.hasOwnProperty("distanceMeters")) {
          if (route.distanceMeters / 1000 >= 1) {
            content += "<p>Distance: " + route.distanceMeters / 1000 + "KM</p>";
          } else {
            content += "<p>Distance: " + route.distanceMeters + "M</p>";
          }
        }
        if (route.hasOwnProperty("duration")) {
          let duration = route.duration.slice(0, -1);
          if (duration / 60 >= 1) {
            content += "<p>Duration: " + Math.round(duration / 60) + "min</p>";
          } else {
            content += "<p>Duration: " + route.duration + "</p>";
          }
        }
        if (route.hasOwnProperty("travelAdvisory") && route.travelAdvisory.hasOwnProperty("fuelConsumptionMicroliters")) {
          if (route.travelAdvisory.fuelConsumptionMicroliters / 1000 >= 1) {
            content += "<p>Fuel consumption: " + route.travelAdvisory.fuelConsumptionMicroliters / 1000 + "L</p>";
          } else {
            content += "<p>Fuel consumption: " + route.travelAdvisory.fuelConsumptionMicroliters + "ML</p>";
          }
        }
        content += "</div>";
        routeTag.innerHTML = content;
        let markerView = new google.maps.marker.AdvancedMarkerView({
          map: myMapObj.gMap,
          position: { lat: location.lat(), lng: location.lng() },
          zIndex: 1,
          content: routeTag
        });
        myMapObj.routeLabels.push(markerView);
        markerView.addEventListener("gmp-click", (e) => {
        });
      }

      routes.forEach((route) => {
        if (route.travelAdvisory && route.travelAdvisory.speedReadingIntervals) {
          speedIntervals.push(route.travelAdvisory.speedReadingIntervals);
        }
        if (route.hasOwnProperty("polyline")) {
          let routePath = google.maps.geometry.encoding.decodePath(route.polyline.encodedPolyline);
          decodedPaths.push(routePath);

          // midPoint is the location used for labelling routes
          let midPoint = parseInt(routePath.length / 2);
          addRouteLabel(routePath[midPoint], route);
        } else {
          myMapObj.setErrorMsg("Something wrong happened while fetching the polyline. Please try again later.")
        }
      });

      this.wayPoints.push(this.addMarker(new google.maps.LatLng({ lat: routes[0].legs[0].startLocation.latLng.latitude, lng: routes[0].legs[0].startLocation.latLng.longitude }), 'A'));
      this.wayPoints.push(this.addMarker(new google.maps.LatLng({ lat: routes[0].legs[0].endLocation.latLng.latitude, lng: routes[0].legs[0].endLocation.latLng.longitude }), 'B'));

      if (speedIntervals.length > 0) {
        for (let i = decodedPaths.length - 1; i >= 0; i--) {
          let decodedPath = decodedPaths[i];
          let speedInterval = speedIntervals[i];
          let polyline = {};

          speedInterval.forEach(function (item, j) {
            let section = [];

            if (item.startPolylinePointIndex == undefined) {
              item.startPolylinePointIndex = 0; //inject into returned json
            }

            for (let p = item.startPolylinePointIndex; p <= item.endPolylinePointIndex; p++) {
              section.push(new google.maps.LatLng(decodedPath[p].lat(), decodedPath[p].lng()));
            }

            polyline = new google.maps.Polyline({
              map: myMapObj.gMap,
              path: section,
              strokeColor: colors[item.speed],
              strokeOpacity: i == 0 ? 0.8 : 0.4,
              strokeWeight: 5
            });
            myMapObj.paths.push(polyline);
          });
        }
      }
      else {
        for (let i = decodedPaths.length - 1; i >= 0; i--) {
          let polyline = new google.maps.Polyline({
            map: myMapObj.gMap,
            path: decodedPaths[i],
            strokeColor: i == 0 ? colors.NORMAL : colors.ALTERNATIVE,
            strokeOpacity: 0.8,
            strokeWeight: 5
          });
          myMapObj.paths.push(polyline);
        }
      }
      this.setViewPort(routes[0].viewport);
    },
    setViewPort: function (viewPort) {
      let sw = new google.maps.LatLng({ lat: viewPort.low.latitude, lng: viewPort.low.longitude });
      let ne = new google.maps.LatLng({ lat: viewPort.high.latitude, lng: viewPort.high.longitude });
      let bounds = new google.maps.LatLngBounds(sw, ne);
      this.gMap.fitBounds(bounds);
    },
    addMarker: function (pos, label) {
      let pinGlyph = new google.maps.marker.PinElement({
        glyphColor: "#fff",
        glyph: label
      });
      let marker = new google.maps.marker.AdvancedMarkerElement({
        position: pos,
        gmpDraggable: true,
        content: pinGlyph.element,
        map: this.gMap
      });
      marker.metadata = { id: label };

      marker.addListener("dragend", function (e) {
        if (this.metadata.id == "A") {
          let originCheckbox = document.getElementById("origin_input_toggle");
          if (!originCheckbox.checked) {
            originCheckbox.click(); // Enforce lat/lng as waypoint
          }
          document.getElementById("origin").value = e.latLng.lat().toFixed(3) + "," + e.latLng.lng().toFixed(3);
          myMapObj.sendRequest();
        } else {
          let desCheckbox = document.getElementById("des_input_toggle");
          if (!desCheckbox.checked) {
            desCheckbox.click(); // Enforce lat/lng as waypoint
          }
          document.getElementById("destination").value = e.latLng.lat().toFixed(3) + "," + e.latLng.lng().toFixed(3);
          myMapObj.sendRequest();
        }
      });
      return marker;
    },
    clearUI: function (obj, type) {
      if (obj.length > 0) {
        if (type == 'advMarker') {
          obj.forEach(function (item) {
            item.map = null;
            // item.setMap(null);
          });
        } else {
          obj.forEach(function (item) {
            item.setMap(null);
          });
        }
      }
    }
  };

  function initMap() {
    myMapObj.init();
  }

  window.initMap = initMap;
}());