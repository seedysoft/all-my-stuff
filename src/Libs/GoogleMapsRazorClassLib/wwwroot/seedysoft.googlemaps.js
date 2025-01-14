window.seedysoft = window.seedysoft || {};
window.seedysoft.googleMaps = window.seedysoft.googleMaps || {};
window.seedysoft.scriptLoader = window.seedysoft.scriptLoader || {};

window.seedysoft.googleMaps = {
  instanceArray: {},

  initialize: (elementId, zoom, center, markers, clickable, dotNetHelper) => {
    let mapOptions = {
      center: center,
      disableDefaultUI: true,
      mapId: elementId,
      zoom: zoom
    };

    let map = new google.maps.Map(document.getElementById(elementId), mapOptions);

    window.seedysoft.googleMaps.instanceArray[elementId] = {
      center: center,
      clickable: clickable,
      directionsRenderer: new google.maps.DirectionsRenderer({
        draggable: false,
        map: map
      }),
      directionsService: new google.maps.DirectionsService(),
      infoWindow: new google.maps.InfoWindow({
        content: "",
        disableAutoPan: true
      }),
      map: map,
      markerArray: markers,
      zoom: zoom
    };

    if (markers) {
      for (const marker of markers) {
        window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
      }
    }
  },
  get: (elementId) => {
    return window.seedysoft.googleMaps.instanceArray[elementId];
  },

  addMarker: (elementId, marker, dotNetHelper) => {
    let mapInstance = window.seedysoft.googleMaps.get(elementId);

    let map = mapInstance.map;
    let clickable = mapInstance.clickable;
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
        background: marker.pinElement.background,
        borderColor: marker.pinElement.borderColor,
        glyph: _glyph,
        glyphColor: marker.pinElement.glyphColor,
        scale: marker.pinElement.scale,
      });
      _content = pin.element;
    }
    else if (marker.content) {
      _content = document.createElement("div");
      _content.classList.add("bb-google-marker-content");
      _content.innerHTML = marker.content;
    }

    const markerEl = new google.maps.marker.AdvancedMarkerElement({
      map,
      content: _content,
      position: marker.position,
      title: marker.title,
      gmpClickable: clickable
    });

    mapInstance.markerArray.push(markerEl);

    // add a click listener for each marker, and set up the info window.
    if (clickable) {
      markerEl.addListener("click", () => {
        window.seedysoft.openInfoWindow(elementId, marker, dotNetHelper);
      });
    }
  },
  removeAllMarkers: (elementId) => {
    let mapInstance = window.seedysoft.googleMaps.get(elementId);

    if (mapInstance.markerArray.length > 0) {
      for (const markerEl of mapInstance.markerArray) {
        markerEl.setMap(null);
      }
      mapInstance.markerArray = [];
    }
  },
  //updateMarkers: (elementId, markers, dotNetHelper) => {
  //  window.seedysoft.googleMaps.removeAllMarkers(elementId);

  //  if (markers) {
  //    for (const marker of markers) {
  //      window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
  //    }
  //  }
  //},

  openInfoWindow: (elementId, marker, dotNetHelper) => {
    let mapInstance = window.seedysoft.googleMaps.get(elementId);
    mapInstance.map.panTo(marker.position);
    mapInstance.map.setZoom(14);

    mapInstance.infoWindow.close();
    mapInstance.infoWindow.setContent(marker.title);
    mapInstance.infoWindow.open(mapInstance.map, marker.position);
    dotNetHelper.invokeMethodAsync("OnClickGoogleMapMarkerJS", marker);
  },

  directionsRoute: (elementId, request) => {
    let mapInstance = window.seedysoft.googleMaps.get(elementId);
    mapInstance.directionsRenderer.setDirections(null);

    return mapInstance.directionsService
      .route(request)
      .then((response) => {
        mapInstance.directionsRenderer.setDirections(response);

        if (response.routes.length > 1) {
          mapInstance.directionsRenderer.setRouteIndex(-1);
        }

        return response.routes.map((route, index) => {
          return {
            index: index,
            summary: route.summary,
            distance: route.legs[0].distance.text,
            duration: route.legs[0].duration.text,
            warnings: route.warnings
          };
        });
      })
      .catch((e) => { window.alert(`Directions request failed: '${e}'`); });
  },

  highlightRoute: (elementId, routeIndex, dirLeg) => {
    let mapInstance = window.seedysoft.googleMaps.get(elementId);
    mapInstance.directionsRenderer.setRouteIndex(routeIndex);

    var directionsResult = mapInstance.directionsRenderer.getDirections();
    var highlightedDirectionsRoute = directionsResult.routes[routeIndex];
    // A route with no stopover waypoints will contain one DirectionsLeg and a route with one stopover waypoint will contain two.
    var directionsLeg = highlightedDirectionsRoute.legs[0];

    dirLeg = JSON.stringify(highlightedDirectionsRoute.legs[0]);
    return dirLeg;
    var directionsSteps = directionsLeg.steps;

    //return JSON.stringify(directionsSteps);
    // Array<LatLng>
    //return directionsSteps.map((step) => {
    //  return { path: step.path };
    //});
  }
}

window.seedysoft.scriptLoader = {
  initialize: (elementId, async, defer, scriptId, source, type, dotNetHelper) => {
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
