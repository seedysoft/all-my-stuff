import * as L from '../lib/leaflet/leaflet.js';
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

    if (popupContent)
        marker.bindPopup(DOMPurify.sanitize(popupContent, { USE_PROFILES: { html: true } }));

    marker.addTo(map);
}

export function addCircleMarker(circleOptions, popup, tooltip) {
    // debugger
    const circleMarker = new L.CircleMarker(circleOptions.position, circleOptions);

    if (typeof popup === 'string') {
        circleMarker.bindPopup(DOMPurify.sanitize(popup, { USE_PROFILES: { html: true } }));
    } else if (typeof popup === 'object' && 'content' in popup) {
        circleMarker.bindPopup(DOMPurify.sanitize(popup.content, { USE_PROFILES: { html: true } }), popup);
    }

    if (typeof tooltip === 'string') {
        circleMarker.bindTooltip(DOMPurify.sanitize(tooltip, { USE_PROFILES: { html: true } }));
    } else if (typeof tooltip === 'object' && 'content' in tooltip) {
        circleMarker.bindTooltip(DOMPurify.sanitize(tooltip.content, { USE_PROFILES: { html: true } }), tooltip);
    }

    circleMarker.addTo(map);
}

export function removeAllMarkers() {
    // debugger
    map.eachLayer(function (l) {
        if (l.options.pane == 'overlayPane' || l.options.pane == 'markerPane')
            l.remove();
    })
}

export function loadProductLimits(productLimits) {
    debugger
    // https://leafletjs.com/examples/layers-control/
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
