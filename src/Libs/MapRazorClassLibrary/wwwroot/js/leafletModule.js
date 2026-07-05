// import * as L from "../lib/leaflet/dist/leaflet.js"
// import * as L from "../lib/leaflet/src/Leaflet.js";

const maps = new Map(); // new Dictionary<string, object> in c#

const createMap = (mapId, point, zoomLevel) => {
    console.log(point);
    let map = L.map(mapId);
    if (point) {
        map.setView([point.latitude, point.longitude], zoomLevel);
    }
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: zoomLevel,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright" target="_blank">OpenStreetMap</a>'
    }).addTo(map);
    map.addedMarkers = [];      // Attibuto nuevo personalizado
    maps.set(mapId, map);

    /*
    Otra forma de guardar el objecto map:
    var element = document.getElementById(mapId);
    element.Map = map;
    */

    // Cuando el mapa se crea dentro de un contenedor oculto o que aun se esta mostrando
    // (un modal, un panel colapsado, etc.) Leaflet cachea un tamano 0x0 y las teselas salen
    // grises/parciales. Forzamos un recalculo en el primer frame y observamos el contenedor
    // para reajustar en cuanto adquiere tamano real. invalidateSize() es un no-op cuando el
    // tamano no cambia, asi que es seguro y retrocompatible.
    const container = document.getElementById(mapId);
    if (container) {
        requestAnimationFrame(() => map.invalidateSize());
        if (typeof ResizeObserver !== 'undefined') {
            let lastW = container.clientWidth, lastH = container.clientHeight;
            const ro = new ResizeObserver(() => {
                const w = container.clientWidth, h = container.clientHeight;
                if (w > 0 && h > 0 && (w !== lastW || h !== lastH)) {
                    lastW = w; lastH = h;
                    map.invalidateSize();
                }
            });
            ro.observe(container);
            map._sizeObserver = ro;
        }
    }

    console.info(`map ${mapId} created.`);
}

const deleteMap = (mapId) => {
    let map = maps.get(mapId);
    maps.delete(mapId);
    if (map._sizeObserver) {
        map._sizeObserver.disconnect();
        map._sizeObserver = null;
    }
    map.remove();
    console.info(`map ${mapId} removed.`);
}

const setView = (mapId, point, zoomLevel) => {
    let map = maps.get(mapId);
    map.setView([point.latitude, point.longitude], zoomLevel);
}

const addMarkerWithOptions = (mapId, point, description, options) => {
    let map = maps.get(mapId);
    let marker = L.marker([point.latitude, point.longitude], options)
        .bindPopup(description)
        .addTo(map);
    let marketId = map.addedMarkers.push(marker) - 1;
    return marketId;       // Devuelve el indice del elemento insertado
}

const buildMarkerOptions = (title, iconUrl, draggable) => {
    let options = {
        title: title
    }
    if (iconUrl) {
        options.icon = L.icon({ iconUrl: iconUrl, iconSize: [32, 32], iconAnchor: [16, 16] });
    }
    if (draggable) {
        options.draggable = draggable
    }
    return options;
}

const addMarker = (mapId, point, title, description, iconUrl) =>
    addMarkerWithOptions(mapId, point, description, buildMarkerOptions(title, iconUrl));

// const addDraggableMarker = (mapId, point, title, description, iconUrl) => {
//     let options = buildMarkerOptions(title, iconUrl, true);
//     let markerId = addMarkerWithOptions(mapId, point, description, options);
//     let map = maps.get(mapId);
//     let marker = GetMarker(mapId, markerId);
//     marker.on('dragend', function (event) {
//         let marker = event.target;
//         let position = marker.getLatLng();
//         let point = {
//             MarkerId: markerId,
//             Point: {
//                 Latitude: position.lat,
//                 Longitude: position.lng
//             }
//         };
//         let dotNet = map.markerHelper.dotNetObjectReference;
//         let dragendHandler = map.markerHelper.dragendHandler;
//         if (dotNet !== null && dotNet !== undefined)
//             dotNet.invokeMethodAsync(dragendHandler, point);
//         else
//             console.warn("Can't connect with the app.");
//     });
//     return markerId;       // Devuelve el indice del elemento insertado
// }

const setMarkerHelper = (mapId, dotNet, dragendHandler) => {
    let map = maps.get(mapId);
    map.markerHelper = {
        dotNetObjectReference: dotNet,
        dragendHandler: dragendHandler
    };
}

const removeMarkers = (mapId) => {
    let map = maps.get(mapId);
    map.addedMarkers.forEach(marker => marker.removeFrom(map));
    map.addedMarkers = [];
}

const drawCircle = (mapId, center, color, fillColor, fillOpacity, radius) => {
    let map = maps.get(mapId);
    L.circle([center.latitude, center.longitude], {
        color: color,
        fillColor: fillColor,
        fillOpacity: fillOpacity,
        radius: radius
    }).addTo(map);
    //optionalmente, guardar el circulo
}

const moveMarker = (mapId, markerId, newPoint) => {
    GetMarker(mapId, markerId)
        .setLatLng([newPoint.latitude, newPoint.longitude]);
}

const setPopupMarkerContent = (mapId, markerId, content) => {
    GetMarker(mapId, markerId)
        .setPopupContent(content);
}

const GetMarker = (mapId, markerId) =>
    maps.get(mapId).addedMarkers[markerId];

// Funcion para geocoding inverso (obtener direccion desde coordenadas)
const reverseGeocode = async (lat, lng) => {
    try {
        const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`);
        const data = await response.json();

        if (data && data.address) {
            const address = data.display_name || '';
            const addressDetails = {
                StreetNumber: data.address.house_number || '',
                Route: data.address.road || '',
                Neighborhood: data.address.neighbourhood || data.address.suburb || '',
                Locality: data.address.city || data.address.town || data.address.village || '',
                AdministrativeArea: data.address.state || '',
                PostalCode: data.address.postcode || '',
                Country: data.address.country || ''
            };

            return { address, addressDetails };
        }
    } catch (error) {
        console.warn('Error en reverse geocoding:', error);
    }

    return { address: '', addressDetails: null };
}

// Funcion para verificar si el click fue sobre un marker
const findClickedMarker = (map, latlng) => {
    for (let i = 0; i < map.addedMarkers.length; i++) {
        const marker = map.addedMarkers[i];
        const markerLatLng = marker.getLatLng();
        const distance = markerLatLng.distanceTo(latlng);
        // Si la distancia es menor a 5 metros, consideramos que se hizo click en el marker
        if (distance < 5) {
            return {
                markerId: marker.markerId,
                marker: marker
            };
        }
    }
    return null;
}

const setClickHandler = async (mapId, dotNet, clickHandlerMethod) => {
    let map = maps.get(mapId);
    if (map.clickHandler) {
        map.off('click', map.clickHandler);
    }
    map.clickHandler = async function (event) {
        const lat = event.latlng?.lat ?? 0;
        const lng = event.latlng?.lng ?? 0;

        // Verificar si se hizo click en un marker
        const clickedMarker = findClickedMarker(map, event.latlng);
        const markerId = clickedMarker ? clickedMarker.markerId.toString() : null;

        // Obtener informacion de geocoding
        let geocodeInfo = { address: '', addressDetails: null };
        if (lat !== 0 || lng !== 0) { // evita hacer reverse geocoding en 0,0 si no es necesario
            try {
                geocodeInfo = await reverseGeocode(lat, lng);
            } catch (err) {
                console.warn('Error en reverseGeocode:', err);
            }
        }

        if (dotNet && clickHandlerMethod) {
            dotNet.invokeMethodAsync(clickHandlerMethod, lat, lng, geocodeInfo.address, geocodeInfo.addressDetails, markerId);
        }

        return { lat, lng, markerId, geocodeInfo };
    };

    map.on('click', map.clickHandler);
    console.info(`Click handler added to map ${mapId}`);
}

// const enableSpinner = (mapId) => {
//     maps.get(mapId).spin(true);
// }
// const disableSpinner = (mapId) => {
//     maps.get(mapId).spin(false);
// }

const addDirection = e => {
  _calculateArrowLines(JSON.parse(e.polyline), e.start, e.radianAngle, e.length, e.symbol, map, LeafletCore)
};
const route = e => {
    _route(e, map, LeafletCore, _dotNetObjRef)
};

export {
    createMap
    , deleteMap
    , setView
    , addMarker
    , addMarkerWithOptions
    // , addDraggableMarker
    , removeMarkers
    , drawCircle
    , moveMarker
    , buildMarkerOptions
    , setMarkerHelper
    , setPopupMarkerContent
    , setClickHandler
    // , enableSpinner
    // , disableSpinner
    , addDirection
    , route
}
