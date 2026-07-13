let maps = {};

export function createMap(id, options) {
    const map = new L.Map(id).setView(options.center, options.zoom);
    const tiles = new L.TileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    maps[id] = map;

    return map;
}

export function destroyMap(id) {
    debugger
    maps[id].remove();
}

export function addMarker(id, markerOptions, iconOptions, popupContent) {
    // debugger
    const map = maps[id];
    
    const marker = new L.Marker(markerOptions.latLng, markerOptions)
        .setIcon(new L.Icon(iconOptions))
        .addTo(map);

    if (popupContent)
        marker.bindPopup(popupContent);
}

export function removeAllMarkers(id) {
    debugger
    const map = maps[id];

    // const overlay = map.getPane('overlayPane');
    // marker.remove();

    // const overlay = map.getPane('markerPane');
}

// export function setView(id, lat, lng, zoom) {
//     debugger
//     maps[id].setView([lat, lng], zoom);
// }

export function registerClick(id, dotnetObj) {
    debugger
    maps[id].on('click', function (e) {
        dotnetObj.invokeMethodAsync("OnMapClickAsync", {
            latLng: e.latlng
        });
    });
}

export function addPolyline(id, route) {
    // debugger
    const map = maps[id];
    const polyline = new L.Polyline(route.points, route.options);
    //.addTo(map);

    // zoom the map to the polyline
    map.fitBounds(polyline.getBounds());
}
