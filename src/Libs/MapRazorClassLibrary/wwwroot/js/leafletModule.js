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

export function addMarker(id, latLng, options) {
    const map = maps[id];
    const marker = new Marker([latLng, options]).addTo(map);
    // if (popupText) marker.bindPopup(popupText);
}

export function removeAllMarkers(id) {
    //debugger
    //maps[id].eachLayer(removeLayer);
}

export function setView(id, lat, lng, zoom) {
    debugger
    maps[id].setView([lat, lng], zoom);
}

// export function addPolygon(id, points) {
//     L.polygon(points).addTo(maps[id]);
// }

export function registerClick(id, dotnetObj) {
    debugger
    maps[id].on('click', function (e) {
        dotnetObj.invokeMethodAsync("OnMapClick", {
            latLng: e.latlng
        });
    });
}

export function addPolyline(id, route) {
    debugger
    const map = maps[id];
    const polyline = new L.Polyline(route.points, route.options).addTo(map);

    // zoom the map to the polyline
    map.fitBounds(polyline.getBounds());
}
