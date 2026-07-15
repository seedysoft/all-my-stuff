import * as L from '../lib/leaflet/dist/leaflet.js';
import { LoaderControl } from './leaflet-loader.js';

var map;
var controlLoader;

export function createMap(id, options) {
    //debugger
    map = new L.Map(id).setView(options.center, options.zoom);
    const tiles = new L.TileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    controlLoader = new LoaderControl().addTo(map);

    map.on('dragend', function () {
        controlLoader.show();
        setTimeout(function () {
            controlLoader.hide();
        }, 3000);
    });

    return map;
}

export function destroyMap() {
    debugger
    map.remove();
}

export function addMarker(markerOptions, iconOptions, popupContent) {
    // debugger
    const marker = new L.Marker(markerOptions.position, markerOptions);
    if (iconOptions)
        marker.setIcon(new L.Icon(iconOptions));
    else
        marker.setIcon(new L.Icon.Default);
    marker.addTo(map);

    if (popupContent)
        marker.bindPopup(popupContent);
}

export function removeAllMarkers() {
    // debugger
    map.eachLayer(function (l) {
        if (l.options.pane == 'overlayPane' || l.options.pane == 'markerPane')
            l.remove();
    })
}

// export function setView(lat, lng, zoom) {
//     debugger
//     map.setView([lat, lng], zoom);
// }

export function registerClick(dotnetObj) {
    debugger
    map.on('click', function (e) {
        dotnetObj.invokeMethodAsync("OnMapClickAsync", {
            latLng: e.latlng
        });
    });
}

export function addPolyline(route) {
    // debugger
    const polyline = new L.Polyline(route.points, route.options)
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
