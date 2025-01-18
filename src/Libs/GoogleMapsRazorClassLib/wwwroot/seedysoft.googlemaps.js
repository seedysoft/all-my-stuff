window.seedysoft = window.seedysoft || {};
window.seedysoft.googleMaps = window.seedysoft.googleMaps || {};
window.seedysoft.scriptLoader = window.seedysoft.scriptLoader || {};

window.seedysoft.googleMaps = {
  instanceArray: {},

  init: (elementId, zoom, center, markers, clickable, dotNetHelper) => {
    let mapContainer = document.getElementById(elementId);

    let mapOptions = {
      center: center,
      disableDefaultUI: true,
      mapId: elementId,
      zoom: zoom
    }

    let gMap = new google.maps.Map(mapContainer, mapOptions);

    let mapWrapper = {
      center: center,
      clickable: clickable,
      directionsRenderer: new google.maps.DirectionsRenderer({
        draggable: false,
        map: gMap
      }),
      directionsService: new google.maps.DirectionsService(),
      infoWindow: new google.maps.InfoWindow({
        content: "",
        disableAutoPan: true
      }),
      gMap: gMap,
      markerArray: markers,
      zoom: zoom
    };
    window.seedysoft.googleMaps.instanceArray[elementId] = mapWrapper;

    if (markers) {
      for (const marker of markers) {
        window.seedysoft.googleMaps.addMarker(elementId, marker, dotNetHelper);
      }
    }

    //window.seedysoft.googleMaps.mapEvents(mapWrapper);
  },
  get: (elementId) => {
    return window.seedysoft.googleMaps.instanceArray[elementId];
  },
  mapEvents: (mapWrapper) => {

    function clickGetLatLng() {
      //let infowindowContainer = document.getElementById("infowindow-alert");
      //let infoWindow = new google.maps.InfoWindow();
      //let infoWindowAlert = document.getElementById("infowindow-alert");

      //mapWrapper.gMap.addListener('click', (mapsMouseEvent) => {
      //  let location = mapsMouseEvent.latLng.lat() + "," + mapsMouseEvent.latLng.lng();
      //  infoWindowAlert.style.visibility = "visible";
      //  infoWindow.close();
      //  infoWindow.setContent(infowindowContainer);
      //  infoWindow.setPosition({ lat: mapsMouseEvent.latLng.lat(), lng: mapsMouseEvent.latLng.lng() });
      //  navigator.clipboard.writeText(location).then(function () {
      //    infoWindow.open(myMapObj.gMap);
      //    setTimeout(function () { infoWindow.close() }, 2000);
      //  }, function () {

      //  });
      //});
    }

    function inputAutocomplete() {
      let inputsAutocomplete = document.querySelectorAll(".gAutocomplete input");
      let autocomplete = [];
      let autocompleteLsr = [];

      function autoComplete(elem, i) {
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
      }

      inputsAutocomplete.forEach((inputField, index) => {
        // Address as waypoint and Autocomplete are enabled by default
        autoComplete(inputField, index);
      });
    }

    //clickGetLatLng();
    //inputAutocomplete();
  },

  addMarker: (elementId, marker, dotNetHelper) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    let gMap = mapWrapper.gMap;
    let clickable = mapWrapper.clickable;
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
      gMap,
      content: _content,
      position: marker.position,
      title: marker.title,
      gmpClickable: clickable
    });

    mapWrapper.markerArray.push(markerEl);

    // add a click listener for each marker, and set up the info window.
    if (clickable) {
      markerEl.addListener("click", () => {
        window.seedysoft.openInfoWindow(elementId, marker, dotNetHelper);
      });
    }
  },
  removeAllMarkers: (elementId) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    if (mapWrapper.markerArray.length > 0) {
      for (const markerEl of mapWrapper.markerArray) {
        markerEl.setMap(null);
      }
      mapWrapper.markerArray = [];
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
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);
    mapWrapper.gMap.panTo(marker.position);
    mapWrapper.gMap.setZoom(14);

    mapWrapper.infoWindow.close();
    mapWrapper.infoWindow.setContent(marker.title);
    mapWrapper.infoWindow.open(mapWrapper.gMap, marker.position);
    dotNetHelper.invokeMethodAsync("OnClickGoogleMapMarkerJS", marker);
  },

  directionsRoute: (elementId, response) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);

    mapWrapper.directionsRenderer.setDirections(response);

    if (response.routes.length > 1) {
      mapWrapper.directionsRenderer.setRouteIndex(-1);
    } 
  },

  highlightRoute: (elementId, routeIndex) => {
    let mapWrapper = window.seedysoft.googleMaps.get(elementId);
    mapWrapper.directionsRenderer.setRouteIndex(routeIndex);
  }
}

window.seedysoft.scriptLoader = {
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
