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
          } else {
            reqBody.origin.placeId = origin;
          }
        }
      } else {
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
          } else {
            reqBody.destination.placeId = destination;
          }
        }
      } else {
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
          myMapObj.setErrorMsg("No routes found. It's likely the waypoints location have problems or the travel mode is not supported in this location.");
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
        alert.querySelector("p").innerHTML = msg.trim();
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
          content += "<p>Distance: ";
          if (route.distanceMeters / 1000 >= 1) {
            content += route.distanceMeters / 1000;
          } else {
            content += route.distanceMeters;
          }
          content += "m</p>";
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
      } else {
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

//<script nonce="54nAPDxT80kYV3XYGG8nRx02Z6AT9E">
(function () {
  window.framebox = window.framebox || function () {
    (window.framebox.q = window.framebox.q || []).push(arguments)
  };

  var a = {}, b = function () {
    (window.framebox.dq = window.framebox.dq || []).push(arguments)
  };
  ['getUrl', 'handleLinkClicksInParent', 'initAutoSize', 'navigate', 'pushState', 'replaceState', 'requestQueryAndFragment', 'sendEvent', 'updateSize', 'scrollParentWindow']
    .forEach(function (x) {
      a[x] = function () {
        b(x, arguments)
      }
    });
  window.devsite = {
    framebox: {
      AutoSizeClient: a
    }
  };
})();

(function (d, e, v, s, i, t, E) {
  d['GoogleDevelopersObject'] = i;
  t = e.createElement(v);
  t.async = 1;
  t.src = s;
  E = e.getElementsByTagName(v)[0];
  E.parentNode.insertBefore(t, E);
})(window,
  document,
  'script',
  'https://www.gstatic.com/devrel-devsite/prod/vda41147226ae308b24384f785d31d739107d2716272d99cd11c490ff3892954d/developers/js/app_loader.js',
  '[1,"es",null,"/js/devsite_app_module.js","https://www.gstatic.com/devrel-devsite/prod/vda41147226ae308b24384f785d31d739107d2716272d99cd11c490ff3892954d","https://www.gstatic.com/devrel-devsite/prod/vda41147226ae308b24384f785d31d739107d2716272d99cd11c490ff3892954d/developers","https://developers-dot-devsite-v2-prod.appspot.com",null,1,null,1,null,[1, 6, 8, 12, 14, 17, 21, 25, 50, 52, 63, 70, 75, 76, 80, 87, 91, 92, 93, 97, 98, 100, 101, 102, 103, 104, 105, 107, 108, 109, 110, 112, 113, 117, 118, 120, 122, 124, 125, 126, 127, 129, 130, 131, 132, 133, 134, 135, 136, 138, 140, 141, 147, 148, 149, 151, 152, 156, 157, 158, 159, 161, 163, 164, 168, 169, 170, 179, 180, 182, 183, 186, 191, 193, 196], "AIzaSyAP-jjEJBzmIyKR4F-3XITp8yM9T1gEEI8", "AIzaSyB6xiKGDR5O3Ak2okS4rLkauxGUG7XP0hg", "developers.google.com", "AIzaSyAQk0fBONSGUqCNznf6Krs82Ap1-NV6J4o", "AIzaSyCCxcqdrZ_7QMeLCRY20bh_SXdAYqy70KY", null, null, null, ["Cloud__enable_legacy_calculator_redirect", "TpcFeatures__enable_unmirrored_page_left_nav", "MiscFeatureFlags__enable_variable_operator", "Profiles__enable_page_saving", "Concierge__enable_key_takeaways", "Search__enable_ai_search_summaries_restricted", "Cloud__enable_free_trial_server_call", "MiscFeatureFlags__developers_footer_dark_image", "Cloud__enable_cloud_dlp_service", "Profiles__enable_completecodelab_endpoint", "DevPro__enable_cloud_innovators_plus", "MiscFeatureFlags__enable_view_transitions", "MiscFeatureFlags__enable_project_variables", "Concierge__enable_concierge", "Profiles__enable_complete_playlist_endpoint", "CloudShell__cloud_code_overflow_menu", "Significatio__enable_by_tenant", "Cloud__enable_cloud_facet_chat", "MiscFeatureFlags__enable_firebase_utm", "MiscFeatureFlags__emergency_css", "Search__enable_ai_eligibility_checks", "Cloud__enable_cloudx_experiment_ids", "Search__enable_ai_search_summaries", "Concierge__enable_pushui", "Cloud__enable_cloudx_ping", "Profiles__enable_public_developer_profiles", "CloudShell__cloud_shell_button", "Experiments__reqs_query_experiments", "Profiles__enable_release_notes_notifications", "Profiles__require_profile_eligibility_for_signin", "DevPro__enable_developer_subscriptions", "Profiles__enable_recognition_badges", "Profiles__enable_dashboard_curated_recommendations", "Concierge__enable_concierge_restricted", "Profiles__enable_awarding_url", "MiscFeatureFlags__enable_explain_this_code", "Search__enable_suggestions_from_borg", "TpcFeatures__enable_mirror_tenant_redirects", "Cloud__enable_llm_concierge_chat", "Analytics__enable_clearcut_logging", "Profiles__enable_profile_collections", "Profiles__enable_completequiz_endpoint", "BookNav__enable_tenant_cache_key", "MiscFeatureFlags__developers_footer_image", "Search__enable_page_map", "EngEduTelemetry__enable_engedu_telemetry", "Cloud__enable_cloud_shell", "Search__enable_dynamic_content_confidential_banner", "Cloud__enable_cloud_shell_fte_user_flow", "Profiles__enable_join_program_group_endpoint", "Profiles__enable_developer_profiles_callout"], null, null, "AIzaSyBLEMok-5suZ67qRPzx0qUtbnLmyT_kCVE", "https://developerscontentserving-pa.googleapis.com", "AIzaSyCM4QpTRSqP5qI4Dvjt4OAScIN8sOUlO-k", "https://developerscontentsearch-pa.googleapis.com", 2, 4, null, "https://developerprofiles-pa.googleapis.com", [1, "developers", "Google for Developers", "developers.google.com", null, "developers-dot-devsite-v2-prod.appspot.com", null, null, [1, 1, [1], null, null, null, null, null, null, null, null, [1], null, null, null, null, null, null, [1], [1, null, null, [1, 20], "/recommendations/information"], null, null, null, [1, 1, 1], [1, 1, null, 1, 1]], null, [null, null, null, null, null, null, "/images/lockup-new.svg", "/images/touchicon-180-new.png", null, null, null, null, 1, null, null, null, null, null, null, null, null, 1, null, null, null, "/images/lockup-dark-theme-new.svg", []], [], null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, [6, 1, 14, 15, 20, 22, 23, 29, 32, 36], null, [[null, null, null, [3, 7, 10, 2, 39, 17, 4, 32, 24, 11, 12, 13, 34, 15, 25], null, null, [1, [["docType", "Choose a content type", [["Tutorial", null, null, null, null, null, null, null, null, "Tutorial"], ["Guide", null, null, null, null, null, null, null, null, "Guide"], ["Sample", null, null, null, null, null, null, null, null, "Sample"]]], ["product", "Choose a product", [["Android", null, null, null, null, null, null, null, null, "Android"], ["ARCore", null, null, null, null, null, null, null, null, "ARCore"], ["ChromeOS", null, null, null, null, null, null, null, null, "ChromeOS"], ["Firebase", null, null, null, null, null, null, null, null, "Firebase"], ["Flutter", null, null, null, null, null, null, null, null, "Flutter"], ["Assistant", null, null, null, null, null, null, null, null, "Google Assistant"], ["GoogleCloud", null, null, null, null, null, null, null, null, "Google Cloud"], ["GoogleMapsPlatform", null, null, null, null, null, null, null, null, "Google Maps Platform"], ["GooglePay", null, null, null, null, null, null, null, null, "Google Pay & Google Wallet"], ["GooglePlay", null, null, null, null, null, null, null, null, "Google Play"], ["Tensorflow", null, null, null, null, null, null, null, null, "TensorFlow"]]], ["category", "Choose a topic", [["AiAndMachineLearning", null, null, null, null, null, null, null, null, "AI and Machine Learning"], ["Data", null, null, null, null, null, null, null, null, "Data"], ["Enterprise", null, null, null, null, null, null, null, null, "Enterprise"], ["Gaming", null, null, null, null, null, null, null, null, "Gaming"], ["Mobile", null, null, null, null, null, null, null, null, "Mobile"], ["Web", null, null, null, null, null, null, null, null, "Web"]]]]]], [1, 1], null, 1], [[["UA-24532603-1"], ["UA-22084204-5"], null, null, ["UA-24532603-5"], null, null, [["G-272J68FCRF"], null, null, [["G-272J68FCRF", 2]]], [["UA-24532603-1", 2]], null, [["UA-24532603-5", 2]], null, 1], [[14, 11], [12, 9], [3, 2], [4, 3], [5, 4], [16, 13], [15, 12], [6, 5], [1, 1], [13, 10], [11, 8]], [[1, 1], [2, 2]]],null, 4, null, null, null, null, null, null, null, null, null, null, null, null, null, "developers.devsite.google"],null, "pk_live_5170syrHvgGVmSx9sBrnWtA5luvk9BwnVcvIi7HizpwauFG96WedXsuXh790rtij9AmGllqPtMLfhe2RSwD6Pn38V00uBCydV4m"]')
