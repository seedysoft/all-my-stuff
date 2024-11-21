window.seedysoft = window.seedysoft || {};
window.seedysoft.googleMaps = window.seedysoft.googleMaps || {};
window.seedysoft.scriptLoader = window.window.seedysoft.scriptLoader || {};

window.seedysoft = {
  googleMaps: {
    instances: {},

    create: (elementId, map, zoom, center, markers, clickable) => {
      window.seedysoft.googleMaps.instances[elementId] = {
        map: map,
        zoom: zoom,
        center: center,
        markers: markers,
        clickable: clickable,
        directionsRenderers: [],
        directionsService: new google.maps.DirectionsService()
      };

      return window.seedysoft.googleMaps.get(elementId);
    },
    get: (elementId) => { return window.seedysoft.googleMaps.instances[elementId]; },
    initialize: (elementId, zoom, center, markers, clickable, dotNetHelper) => {
      let mapOptions = { center: center, mapId: elementId, zoom: zoom };

      let map = new google.maps.Map(document.getElementById(elementId), mapOptions);

      let mapInstance = window.seedysoft.googleMaps.create(elementId, map, zoom, center, markers, clickable);

      mapInstance.directionsRenderers.push(new google.maps.DirectionsRenderer({ map: map }));

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

      mapInstance.markers.push(markerEl);

      // add a click listener for each marker, and set up the info window.
      if (clickable) {
        markerEl.addListener("click", ({ domEvent, latLng }) => {
          const { target } = domEvent;
          const infoWindow = new google.maps.InfoWindow();
          infoWindow.close();
          infoWindow.setContent(markerEl.title);
          infoWindow.open(markerEl.map, markerEl);
          dotNetHelper.invokeMethodAsync('OnMarkerClickJS', marker);
        });
      }
    },
    removeAllMarkers: (elementId) => {
      let mapInstance = window.seedysoft.googleMaps.get(elementId);
      // delete the markers
      if (mapInstance.markers.length > 0) {
        for (const markerEl of mapInstance.markers) {
          markerEl.setMap(null);
        }
        mapInstance.markers = [];
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

    directionsRoute: (elementId, request) => {
      let mapInstance = window.seedysoft.googleMaps.get(elementId);

      return mapInstance.directionsService
        .route(request)
        .then((response) => {
          for (var i = 0, len = mapInstance.directionsRenderers.length; i < len; i++) {
            mapInstance.directionsRenderers[i].setMap(null);
            mapInstance.directionsRenderers[i] = null;
          }

          mapInstance.directionsRenderers = [];

          for (var i = 0, len = response.routes.length; i < len; i++) {
            mapInstance.directionsRenderers.push(new google.maps.DirectionsRenderer({
              directions: response,
              map: mapInstance.map,
              routeIndex: i,
              polylineOptions: {
                strokeColor: Colors[i],
                strokeWeight: 5,
                strokeOpacity: 1
              }
            }));
          }

          return mapInstance.map.getBounds();
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
