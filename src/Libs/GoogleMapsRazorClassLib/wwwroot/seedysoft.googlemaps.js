window.seedysoft = window.seedysoft || {};

class MapWrapper {

  constructor(googleMap, isClickable, dotNetHelper) {
    this.gMap = googleMap;
    this.isClickable = isClickable;
    this.dotNetHelper = dotNetHelper;

    this.directionRenderer = new google.maps.DirectionsRenderer({ map: googleMap });
    this.directionsService = new google.maps.DirectionsService();
    this.infoWindow = new google.maps.InfoWindow({ content: "", disableAutoPan: true });
    this.markerArray = [];

    this.paths = [];
    this.routeLabels = [];
    this.wayPoints = [];
    this.viewport = {};

    this.setErrorMsg = function (err) {
      debugger
      let alert = document.getElementById("alert");
      if (typeof err === 'string') {
        alert.querySelector("p").innerHTML = err;
      }
      else if (typeof err === 'object') {
        let msg = err.hasOwnProperty("code") ? err.code : '';
        msg += err.hasOwnProperty("message") ? " " + err.message : '';
        alert.querySelector("p").innerHTML = msg;
      }
      alert.style.display = 'block';
    };

    this.setRoute = function (data) {
      this.clearUI(this.wayPoints, 'marker');
      this.clearUI(this.paths, 'routes');
      this.clearUI(this.routeLabels, 'advMarker');

      let colors = {
        "NORMAL": "#4285f4"
        , "SLOW": "#fbbc04"
        , "TRAFFIC_JAM": "#ea4335"
        , "ALTERNATIVE": "#999999"
      };

      let routes = data.routes; //routes
      let speedIntervals = []; //traffic aware polyline indexes
      let decodedPaths = [];

      function addRouteLabel(mapWrapper, location, route) {
        let routeTag = document.createElement('div');
        let content = '';
        routeTag.className = "route-tag";

        if (route.hasOwnProperty("routeLabels")) {
          content += "<p>";
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
            content += "<p>Distance: " + route.distanceMeters / 1000 + " Km</p>";
          }
          else {
            content += "<p>Distance: " + route.distanceMeters + " m</p>";
          }
        }
        if (route.hasOwnProperty("duration")) {
          let duration = route.duration.slice(0, -1);
          if (duration / 60 >= 1) {
            content += "<p>Duration: " + Math.round(duration / 60) + " min.</p>";
          }
          else {
            content += "<p>Duration: " + route.duration + "</p>";
          }
        }
        if (route.hasOwnProperty("travelAdvisory") && route.travelAdvisory.hasOwnProperty("fuelConsumptionMicroliters")) {
          if (route.travelAdvisory.fuelConsumptionMicroliters / 1000 >= 1) {
            content += "<p>Fuel consumption: " + route.travelAdvisory.fuelConsumptionMicroliters / 1000 + " l</p>";
          }
          else {
            content += "<p>Fuel consumption: " + route.travelAdvisory.fuelConsumptionMicroliters + " μl</p>";
          }
        }
        content += "</div>";
        routeTag.innerHTML = content;

        const markerView = new google.maps.marker.AdvancedMarkerView({
          content: routeTag
          , gmpClickable: mapWrapper.isClickable
          , map: mapWrapper.gMap
          , position: { lat: location.lat(), lng: location.lng() }
          , zIndex: 1
        });
        mapWrapper.routeLabels.push(markerView);
        markerView.addEventListener("gmp-click", (e) => {
          mapWrapper.dotNetHelper.invokeMethodAsync("OnClickGmapRouteJS", route.polyline.encodedPolyline);
        });
      } // addRouteLabel

      routes.forEach((route) => {
        if (route.travelAdvisory && route.travelAdvisory.speedReadingIntervals) {
          speedIntervals.push(route.travelAdvisory.speedReadingIntervals);
        }

        if (route.hasOwnProperty("polyline")) {
          let routePath = google.maps.geometry.encoding.decodePath(route.polyline.encodedPolyline);
          decodedPaths.push(routePath);

          // midPoint is the location used for labelling routes
          let midPoint = parseInt(routePath.length / 2);
          addRouteLabel(this, routePath[midPoint], route);
        }
        else {
          this.setErrorMsg("Something wrong happened while fetching the polyline. Please try again later.");
        }
      });

      let startLocLatLng = routes[0].legs[0].startLocation.latLng;
      let endLocLatLng = routes[0].legs[0].endLocation.latLng;
      this.wayPoints.push(this.addMarker(new google.maps.LatLng({ lat: startLocLatLng.latitude, lng: startLocLatLng.longitude }), 'A'));
      this.wayPoints.push(this.addMarker(new google.maps.LatLng({ lat: endLocLatLng.latitude, lng: endLocLatLng.longitude }), 'B'));

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
              map: this.gMap
              , path: section
              , strokeColor: colors[item.speed]
              , strokeOpacity: i == 0 ? 0.8 : 0.4
              , strokeWeight: 5
            });
            this.paths.push(polyline);
          });
        }
      }
      else {
        for (let i = decodedPaths.length - 1; i >= 0; i--) {
          const polyline = new google.maps.Polyline({
            map: this.gMap
            , path: decodedPaths[i]
            , strokeColor: i == 0 ? colors.NORMAL : colors.ALTERNATIVE
            , strokeOpacity: 0.8
            , strokeWeight: 5
          });
          this.paths.push(polyline);
        }
      }
      this.viewport = routes[0].viewport;
      this.setViewport(this.viewport);
    };

    this.setViewport = function (viewport) {
      const sw = new google.maps.LatLng({ lat: viewport.low.latitude, lng: viewport.low.longitude });
      const ne = new google.maps.LatLng({ lat: viewport.high.latitude, lng: viewport.high.longitude });
      const bounds = new google.maps.LatLngBounds(sw, ne);
      this.gMap.fitBounds(bounds);
    };

    this.addMarker = function (pos, label) {
      const pinGlyph = new google.maps.marker.PinElement({
        glyph: label
        , glyphColor: "#fff"
      });
      const marker = new google.maps.marker.AdvancedMarkerElement({
        content: pinGlyph.element
        , gmpDraggable: true
        , map: this.gMap
        , position: pos
      });
      marker.metadata = { id: label };

      //marker.addListener("dragend", function (e) {
      //  if (this.metadata.id == "A") {
      //    let originCheckbox = document.getElementById("origin_input_toggle");
      //    if (!originCheckbox.checked) {
      //      originCheckbox.click(); // Enforce lat/lng as waypoint
      //    }
      //    document.getElementById("origin").value = e.latLng.lat().toFixed(3) + "," + e.latLng.lng().toFixed(3);
      //    myMapObj.sendRequest();
      //  }
      //  else {
      //    let desCheckbox = document.getElementById("des_input_toggle");
      //    if (!desCheckbox.checked) {
      //      desCheckbox.click(); // Enforce lat/lng as waypoint
      //    }
      //    document.getElementById("destination").value = e.latLng.lat().toFixed(3) + "," + e.latLng.lng().toFixed(3);
      //    myMapObj.sendRequest();
      //  }
      //});

      return marker;
    };

    this.clearUI = function (obj, type) {
      if (obj.length > 0) {
        if (type == 'advMarker') {
          obj.forEach(function (item) { item.map = null });
        }
        else {
          obj.forEach(function (item) { item.setMap(null) });
        }
      }
    };
  }
}; // class MapWrapper

window.seedysoft.googleMaps = window.seedysoft.googleMaps || {
  mapWrappers: []

  , init: (elementId, zoom, center, isClickable, dotNetHelper) => {
    let mapOptions = {
      center: center
      , disableDefaultUI: true
      , mapId: elementId
      , zoom: zoom
    };

    const map = new google.maps.Map(document.getElementById(elementId), mapOptions);

    window.seedysoft.googleMaps.mapWrappers[elementId] = new MapWrapper(map, isClickable, dotNetHelper);
  }

  , get: (elementId) => { return window.seedysoft.googleMaps.mapWrappers[elementId]; }

  , searchRoutes: (elementId, ori, des, apikey) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    let origin = ori /*document.getElementById("origin").value.trim()*/
      , destination = des /*document.getElementById("destination").value.trim()*/
      //, heading_org = document.getElementById("heading_org").value.trim()
      //, heading_des = document.getElementById("heading_des").value.trim()
      , routing_preference = 'traffic_unaware' /*document.getElementById("traffic").value 
      <option value="traffic_unaware">Traffic unaware</option>
      <option value="traffic_aware">Traffic aware</option>
      <option value="traffic_aware_optimal">Traffic aware optimal (best routes with accurate ETA)</option>
      */
      , traffic_aware_polyline = false /*document.getElementById("traffic_aware_polyline").checked*/
      , eco_routes = false /*document.getElementById("eco_routes").checked*/
      , travel_mode = "drive" /*document.getElementById("travel_mode").value*/
      , departure_time = '' /*document.getElementById("departuretime").value*/
      ;

    let requestedReferenceRoutes = [];
    let extraComputations = [];
    //let allowedTravelModes = [];

    /******  Start of Request *****/
    let reqBody = {
      "origin": {
        "sideOfRoad": false /*document.getElementById("side_of_road_org").checked*/
        , "vehicleStopover": false /*document.getElementById("origin_stopover").checked*/
      },
      "destination": {
        "sideOfRoad": false /*document.getElementById("side_of_road_des").checked*/
        , "vehicleStopover": false /*document.getElementById("destination_stopover").checked*/
      }
      , "computeAlternativeRoutes": true /*document.getElementById("alternative_routes").checked*/
      , "travelMode": travel_mode
      //, "routingPreference": routing_preference == '' ? "ROUTING_PREFERENCE_UNSPECIFIED" : routing_preference
      //, "polylineQuality": document.getElementById("polyline_quality").value
      //, "routeModifiers": {
      //  "avoidTolls": document.getElementById("avoid_tolls").checked
      //  , "avoidHighways": document.getElementById("avoid_highways").checked
      //  , "avoidFerries": document.getElementById("avoid_ferries").checked
      //  , "avoidIndoor": document.getElementById("avoid_indoor").checked
      //}
    };

    if (origin !== '') {
      //if (mapWrapper.isAddress.origin) {
      reqBody.origin.address = origin;
      //}
      //else {
      //  if (origin.indexOf(',') >= 0) {
      //    origin_lat_lng = origin.split(",");
      //    reqBody.origin.location = {
      //      "latLng": { "latitude": origin_lat_lng[0].trim(), "longitude": origin_lat_lng[1].trim() }
      //    };
      //  }  else {
      //    reqBody.origin.placeId = origin;
      //  }
      //}
    }
    else {
      mapWrapper.setErrorMsg("Origin must be set in a right format.");
      return false;
    }
    if (destination !== '') {
      //if (mapWrapper.isAddress.destination) {
      reqBody.destination.address = destination;
      //}
      //else {
      //  if (destination.indexOf(',') >= 0) {
      //    destination_lat_lng = destination.split(",");
      //    reqBody.destination.location = {
      //      "latLng": { "latitude": destination_lat_lng[0].trim(), "longitude": destination_lat_lng[1].trim() }
      //    };
      //  }
      //  else {
      //    reqBody.destination.placeId = destination;
      //  }
      //}
    }
    else {
      mapWrapper.setErrorMsg("Destination must be set in a right format.");
      return false;
    }

    //if (heading_org && reqBody.origin.location && travel_mode != "transit") {
    //  reqBody.origin.location.heading = heading_org;
    //}
    //if (heading_des && reqBody.destination.location && travel_mode != "transit") {
    //  reqBody.destination.location.heading = heading_des;
    //}

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
        "allowedTravelModes": Array.from(selectedElem).map(x => x.value)
        , "routingPreference": document.getElementById("transitPreference").value
      }
    }

    if (travel_mode != "transit") {
      if (routing_preference == '') {
        reqBody.routingPreference = "ROUTING_PREFERENCE_UNSPECIFIED";
      }
      else {
        reqBody.routingPreference = routing_preference;
      }
    }

    if (departure_time != '') {
      const date = new Date(departure_time);
      let utc_date = `${date.getUTCFullYear()}-${date.getUTCMonth() + 1}-${date.getUTCDate()}T${date.getUTCHours()}:${date.getUTCMinutes()}:00Z`;
      reqBody.departureTime = utc_date;
    }

    fetch("https://routes.googleapis.com/directions/v2:computeRoutes", {
      body: JSON.stringify(reqBody)
      , headers: {
        "Content-Type": "application/json"
        , "X-Goog-Api-Key": apikey
        , "X-Goog-FieldMask": "*"
      }
      , method: "POST"
    }).then((response) => {
      return response.json();
    }).then((data) => {
      if ('error' in data) {
        mapWrapper.setErrorMsg(data.error);
      }
      else if (!data.hasOwnProperty("routes")) {
        mapWrapper.setErrorMsg("No routes found. It's likely the waypoints location have problems or the travel mode is not supported in this location.");
      }
      else {
        mapWrapper.setRoute(data);
      }
    }).catch((error) => { console.log(error); });
    /****** End of Request ******/
  }

  , addGasStationMarker: (elementId, marker, dotNetHelper) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    let map = mapWrapper.gMap;
    let isClickable = mapWrapper.isClickable;
    let _content;

    if (marker.pinElement) {
      let _glyph;

      if (marker.pinElement.useIconFonts) {
        const icon = document.createElement("div");
        icon.innerHTML = `<i class="${marker.pinElement.glyph}"></i>`;
        _glyph = icon;
      }
      else {
        _glyph = marker.pinElement.glyph;
      }

      const pin = new google.maps.marker.PinElement({
        background: marker.pinElement.background
        , borderColor: marker.pinElement.borderColor
        , glyph: _glyph
        , glyphColor: marker.pinElement.glyphColor
        , scale: marker.pinElement.scale
      });
      _content = pin.element;
    }
    else if (marker.content) {
      _content = document.createElement("div");
      _content.classList.add("bb-google-marker-content");
      _content.innerHTML = marker.content;
    }

    const markerEl = new google.maps.marker.AdvancedMarkerElement({
      content: _content
      , gmpClickable: isClickable
      , map: map
      , position: marker.position
      , title: marker.title
    });

    mapWrapper.markerArray.push(markerEl);

    // add a click listener for each marker, and set up the info window.
    if (isClickable) {
      markerEl.addListener("click", () => {
        window.seedysoft.googleMaps.openInfoWindow(elementId, marker, dotNetHelper);
      });
    }
  }
  , removeAllMarkers: (elementId) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    if (mapWrapper.markerArray.length > 0) {
      for (const markerEl of mapWrapper.markerArray) {
        markerEl.setMap(null);
      }
      mapWrapper.markerArray = [];
    }
  }
  //, updateMarkers: (elementId, markers, dotNetHelper) => {
  //  window.seedysoft.googleMaps.removeAllMarkers(elementId);

  //  if (markers) {
  //    for (const marker of markers) {
  //      window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
  //    }
  //  }
  //}

  , openInfoWindow: (elementId, marker, dotNetHelper) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);
    mapWrapper.gMap.panTo(marker.position);
    mapWrapper.gMap.setZoom(14);

    mapWrapper.infoWindow.close();
    mapWrapper.infoWindow.setContent(marker.title);
    mapWrapper.infoWindow.open(mapWrapper.gMap, marker.position);
    dotNetHelper.invokeMethodAsync("OnClickGmapMarkerJS", marker);
  }

  , resetViewport: (elementId) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);
    mapWrapper.setViewport(mapWrapper.viewport);
  }

  //, directionsRoute: (elementId, response) => {
  //  let mapWrapper = window.seedysoft.googleMaps.get(elementId);

  //  mapWrapper.directionsRenderer.setDirections(response);

  //  if (response.routes.length > 1) {
  //    mapWrapper.directionsRenderer.setRouteIndex(-1);
  //  }
  //}

  //, highlightRoute: (elementId, routeIndex) => {
  //  let mapWrapper = window.seedysoft.googleMaps.get(elementId);
  //  mapWrapper.directionsRenderer.setRouteIndex(routeIndex);
  //}
}

window.seedysoft.scriptLoader = window.seedysoft.scriptLoader || {
  init: (elementId, async, defer, scriptId, source, type, dotNetHelper) => {
    if (source.length === 0) {
      console.error("Invalid source url.");
      return;
    }

    let scriptLoaderElement = document.getElementById(elementId);

    if (scriptLoaderElement == null) {
      window.alert(`Cannot find Element ${elementId}`);
    }
    else {
      let scriptElement = document.createElement("script");

      scriptElement.async = async;

      scriptElement.defer = defer;

      if (scriptId != null)
        scriptElement.id = scriptId;

      if (source != null)
        scriptElement.src = source;

      if (type != null)
        scriptElement.type = type;

      scriptElement.addEventListener("error", (_event) => {
        dotNetHelper.invokeMethodAsync("OnErrorJS", `An error occurred while loading the script: ${source}`);
      });

      scriptElement.addEventListener("load", (_event) => {
        dotNetHelper.invokeMethodAsync("OnLoadJS");
      });

      scriptLoaderElement.appendChild(scriptElement);
    }
  }
}
