window.seedysoft = window.seedysoft || {};
window.seedysoft.googleMaps = window.seedysoft.googleMaps || {};
window.seedysoft.scriptLoader = window.seedysoft.scriptLoader || {};

window.seedysoft = {
  googleMaps: {
    instanceArray: {},

    create: (elementId, map, zoom, center, markers, clickable) => {
      window.seedysoft.googleMaps.instanceArray[elementId] = {
        map: map,
        zoom: zoom,
        center: center,
        markerArray: markers,
        clickable: clickable,
        directionRendererArray: [],
        directionsService: new google.maps.DirectionsService(),
        infoWindow: new google.maps.InfoWindow({
          content: "",
          disableAutoPan: true,
        }),
      };

      return window.seedysoft.googleMaps.get(elementId);
    },
    get: (elementId) => {
      return window.seedysoft.googleMaps.instanceArray[elementId];
    },
    initialize: (elementId, zoom, center, markers, clickable, dotNetHelper) => {
      let mapOptions = { center: center, disableDefaultUI: true, mapId: elementId, zoom: zoom };

      let map = new google.maps.Map(document.getElementById(elementId), mapOptions);

      let mapInstance = window.seedysoft.googleMaps.create(elementId, map, zoom, center, markers, clickable);

      mapInstance.directionRendererArray.push(new google.maps.DirectionsRenderer({ map: map }));

      if (markers) {
        for (const marker of markers) {
          window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
        }
      }
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
      // delete the markers
      if (mapInstance.markerArray.length > 0) {
        for (const markerEl of mapInstance.markerArray) {
          markerEl.setMap(null);
        }
        mapInstance.markerArray = [];
      }
    },
    updateMarkers: (elementId, markers, dotNetHelper) => {
      window.seedysoft.googleMaps.removeAllMarkers(elementId);

      if (markers) {
        for (const marker of markers) {
          window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
        }
      }
    },

    openInfoWindow: (elementId, marker, dotNetHelper) => {
      let mapInstance = window.seedysoft.googleMaps.get(elementId);
      mapInstance.map.panTo(marker.position);
      mapInstance.map.setZoom(14);

      mapInstance.infoWindow.close();
      mapInstance.infoWindow.setContent(marker.title);
      mapInstance.infoWindow.open(mapInstance.map, marker.position);
      dotNetHelper.invokeMethodAsync('OnMarkerClickJS', marker);
    },

    directionsRoute: (elementId, request) => {
      let mapInstance = window.seedysoft.googleMaps.get(elementId);

      /*return*/ mapInstance.directionsService
        .route(request)
        .then((response) => {
          for (var i = 0, len = mapInstance.directionRendererArray.length; i < len; i++) {
            mapInstance.directionRendererArray[i].setMap(null);
            mapInstance.directionRendererArray[i] = null;
          }
          mapInstance.directionRendererArray = [];

          // Route the directions and pass the response to a function to create markers for each step.
          let warnings = '';
          document.getElementById("warnings-panel").innerHTML = warnings;

          for (var i = 0, len = response.routes.length; i < len; i++) {
            if (response.routes[i].warnings.length > 0) {
              warnings += "<li><b>" + response.routes[i].warnings + "</b></li>";
            }

            mapInstance.directionRendererArray.push(new google.maps.DirectionsRenderer({
              directions: response,
              map: mapInstance.map,
              routeIndex: i,
              polylineOptions: {
                strokeColor: Colors[i],
                strokeWeight: 5,
                strokeOpacity: 1
              }
            }));

            showSteps(response.routes[i], mapInstance.markerArray, mapInstance.infoWindow, mapInstance.map);
          }

          if (warnings.length > 0) {
            document.getElementById("warnings-panel").innerHTML = "<ul>" + warnings + "</ul>";
          }

          /*return mapInstance.map.getBounds();*/
        })
        .catch((e) => { window.alert("Directions request failed: '" + e + "'"); });
    }
  },

  scriptLoader: {
    initialize: (elementId, async, defer, scriptId, source, type, dotNetHelper) => {
      if (source.length === 0) {
        console.error(`Invalid source url.`);
        return;
      }

      let scriptLoaderElement = document.getElementById(elementId);

      if (scriptLoaderElement == null) {
        window.alert("Cannot find Element " + elementId);
      }
      else {
        let scriptElement = document.createElement('script');

        scriptElement.async = async;

        scriptElement.defer = defer;

        if (scriptId != null)
          scriptElement.id = scriptId;

        if (source != null)
          scriptElement.src = source;

        if (type != null)
          scriptElement.type = type;

        scriptElement.addEventListener("error", (_event) => {
          dotNetHelper.invokeMethodAsync('OnErrorJS', `An error occurred while loading the script: ${source}`);
        });

        scriptElement.addEventListener("load", (_event) => {
          dotNetHelper.invokeMethodAsync('OnLoadJS');
        });

        scriptLoaderElement.appendChild(scriptElement);
      }
    }
  }
}

const Colors = [
  "#004f6f",
  "#031a1f",
  "#4c7c9b",
  "#2a4052",
  "#4d7a85",
  "#234660",
  "#274f58",
  "#005d7d"
];

function showSteps(route, markerArray, stepDisplay, map) {
  //// For each step, place a marker, and add the text to the marker's infowindow.
  //// Also attach the marker to an array so we can keep track of it and remove it when calculating new routes.
  //const myRoute = route.legs[0];

  //for (let i = 0; i < myRoute.steps.length; i++) {
  //  const advancedMarker = new google.maps.marker.AdvancedMarkerElement({
  //    gmpDraggable: false,
  //    map: map,
  //    position: myRoute.steps[i].start_location,
  //  });
  //  markerArray[i] = advancedMarker;

  //  advancedMarker.addEventListener("gmp-click", async () => {
  //    // Open an info window when the marker is clicked on, containing the text of the step.
  //    stepDisplay.setContent(myRoute.steps[i].instructions);
  //    stepDisplay.open(map, marker);
  //  });
  //}
}

//// global function
//invokeMethodAsync: (callbackEventName, dotNetHelper) => {
//    dotNetHelper.invokeMethodAsync(callbackEventName);
//},
//hasInvalidChars: (input, validChars) => {
//    if (input.length <= 0 || validChars.length <= 0)
//        return false;

//    let inputCharArr = input.split('');
//    for (let i = 0; i < inputCharArr.length; i++) {
//        if (!validChars.includes(inputCharArr[i]))
//            return true;
//    }

//    return false;
//},
//scrollToElementBottom: (elementId) => {
//    let el = document.getElementById(elementId);
//    if (el)
//        el.scrollTop = el.scrollHeight;
//},
//scrollToElementTop: (elementId) => {
//    let el = document.getElementById(elementId);
//    if (el)
//        el.scrollTop = 0;
//}
