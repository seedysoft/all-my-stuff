import { Map as LeafletMap, TileLayer, Marker, Icon, CircleMarker, Polyline, GeoJSON, Control, DomUtil, DomEvent } from '../lib/leaflet/leaflet.js';
import { LoaderControl } from './leaflet-loader.js';

let map;
let controlLoader;
let dnRef;

const markersArray = new Map();

export function createMap(id, center, zoom, dotNetRef) {
    //debugger
    map = new LeafletMap(id).setView(center, zoom);
    const tiles = new TileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    controlLoader = new LoaderControl().addTo(map);

    dnRef = dotNetRef;

    map.on('click', function (e) { dnRef.invokeMethodAsync("OnMapClickAsync", { latLng: e.latlng }); });
    map.on('moveend', function () { dnRef.invokeMethodAsync('OnMapMoveEndAsync', map.getBounds()); });

    return map;
}

export function destroyMap() {
    debugger
    map.remove();

    markersArray.forEach(function (m) { m.remove(); });

    map = undefined;
    controlLoader = undefined;
    dnRef = undefined;
}

export function addOrUpdateMarker(markerOptions, iconOptions, popupContent) {
    // debugger
    let marker = markersArray.get(markerOptions.position.key);
    if (marker == undefined) {
        marker = new Marker(markerOptions.position, markerOptions);
        marker.addTo(map);
        markersArray.set(markerOptions.position.key, marker);
    }
    else {
        marker.setStyle(markerOptions);
    }

    if (iconOptions)
        marker.setIcon(new Icon(iconOptions));
    else
        marker.setIcon(new Icon.Default);

    if (popupContent) {
        marker.unbindPopup();
        marker.bindPopup(DOMPurify.sanitize(popupContent, { USE_PROFILES: { html: true } }));
    }
}

export function addOrUpdateCircleMarker(circleOptions, popup, tooltip) {
    // debugger
    let circleMarker = markersArray.get(circleOptions.position.key);
    if (circleMarker == undefined) {
        circleMarker = new CircleMarker(circleOptions.position, circleOptions);
        circleMarker.addTo(map);
        markersArray.set(circleOptions.position.key, circleMarker);
    }
    else {
        circleMarker.setStyle(circleOptions);
    }

    circleMarker.unbindPopup();
    if (typeof popup === 'string')
        circleMarker.bindPopup(DOMPurify.sanitize(popup, { USE_PROFILES: { html: true } }));
    else if (typeof popup === 'object' && 'content' in popup)
        circleMarker.bindPopup(DOMPurify.sanitize(popup.content, { USE_PROFILES: { html: true } }), popup);

    circleMarker.unbindTooltip();
    if (typeof tooltip === 'string')
        circleMarker.bindTooltip(DOMPurify.sanitize(tooltip, { USE_PROFILES: { html: true } }));
    else if (typeof tooltip === 'object' && 'content' in tooltip)
        circleMarker.bindTooltip(DOMPurify.sanitize(tooltip.content, { USE_PROFILES: { html: true } }), tooltip);
}

export function removeGasStations() {
    // debugger
    map.eachLayer(function (l) {
        if (l.options.pane == 'markerPane')
            l.remove();
    })
}
export function removeRoutes() {
    // debugger
    map.eachLayer(function (l) {
        if (l.options.pane == 'overlayPane')
            l.remove();
    })
}

// export function setView(lat, lng, zoom) {
//     debugger
//     map.setView([lat, lng], zoom);
// }

export function addPolyline(route) {
    // debugger
    const polyline = new Polyline(route.points, route.options)
        .addTo(map);

    // zoom the map to the polyline
    map.fitBounds(polyline.getBounds());
}

export function showLoader() {
    // debugger
    controlLoader.show();
}
export function hideLoader() {
    // debugger
    controlLoader.hide();
}
