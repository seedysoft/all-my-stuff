window.seedysoft = window.seedysoft || {};
window.seedysoft.googleMaps = window.seedysoft.googleMaps || {};
window.seedysoft.scriptLoader = window.window.seedysoft.scriptLoader || {};

window.seedysoft = {
  googleMaps: {
    instances: {},
    directionsRenderers: {},

    create: (elementId, map, zoom, center, markers, clickable) => {
      window.seedysoft.googleMaps.instances[elementId] = {
        map: map,
        zoom: zoom,
        center: center,
        markers: markers,
        clickable: clickable
      };
    },
    get: (elementId) => { return window.seedysoft.googleMaps.instances[elementId]; },
    initialize: (elementId, zoom, center, markers, clickable, dotNetHelper) => {
      window.seedysoft.googleMaps.markerEls[elementId] = window.seedysoft.googleMaps.markerEls[elementId] ?? [];

      let mapOptions = { center: center, zoom: zoom, mapId: elementId };
      let map = new google.maps.Map(document.getElementById(elementId), mapOptions);

      window.seedysoft.googleMaps.create(elementId, map, zoom, center, markers, clickable);

      window.seedysoft.googleMaps.directionsRenderers[elementId] = new google.maps.DirectionsRenderer({ map: map });

      if (markers) {
        for (const marker of markers) {
          window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
        }
      }
    },

    markerEls: {},
    addMarker: (elementId, marker, dotNetHelper) => {
      debugger;
      let mapInstance = window.seedysoft.googleMaps.get(elementId);
      if (mapInstance) {
        let map = mapInstance.map;
        let clickable = mapInstance.clickable;
        let _content;

        if (marker.pinElement) {
          let _glyph;

          if (marker.pinElement.useIconFonts) {
            const icon = document.createElement("div");
            icon.innerHTML = `<i class="${marker.pinElement.glyph}"></i>`;
            _glyph = icon;
          } else {
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

        window.seedysoft.googleMaps.markerEls[elementId].push(markerEl);

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
      }
    },
    updateMarkers: (elementId, markers, dotNetHelper) => {
      debugger;
      let markerEls = window.seedysoft.googleMaps.markerEls[elementId] ?? [];

      // delete the markers
      if (markerEls.length > 0) {
        for (const markerEl of markerEls) {
          markerEl.setMap(null);
        }
      }

      if (markers) {
        for (const marker of markers) {
          window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
        }
      }
    },

    directionsRoute: (elementId, request) => {
      let directionsService = new google.maps.DirectionsService();

      return directionsService
        .route(request)
        .then((response) => {
          window.seedysoft.googleMaps.directionsRenderers[elementId].setDirections(response);

          return window.seedysoft.googleMaps.get(elementId).map.getBounds();
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
