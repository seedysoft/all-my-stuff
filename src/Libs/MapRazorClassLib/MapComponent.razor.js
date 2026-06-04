/// reference path="https://componentes.idee.es/api-idee/vendor/browser-polyfill.js"
/// reference path="https://componentes.idee.es/api-idee/js/apiidee.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/js/configuration.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/locator/locator.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/sharemap/sharemap.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/viewmanagement/viewmanagement.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/mousesrs/mousesrs.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/layerswitcher/layerswitcher.ol.min.js"
/// reference path="https://componentes.idee.es/api-idee/plugins/measurebar/measurebar.ol.min.js"

window.seedysoft.maps = window.seedysoft.maps || {
  instances: []
}

// console.log(IDEE.getQuickLayers());
// // Ejemplo cambiando el valor por defecto de IDEE.config.baseLayer
// IDEE.config('baseLayer', 'TMS*PNOA-MA*https://tms-pnoa-ma.idee.es/1.0.0/pnoa-ma/{z}/{x}/{-y}.jpeg*true*false*19');

/**
 * @param {string} elementId  - The name of the div container for the mapjs
 * @param {LngLat} center     - The LngLat where mapjs starts
 * @param {double} zoom       - The zoom level for the mapjs
 * @return {IDEE.Map}         - The mapjs instance
 */
function createMap(elementId, center, zoom) {
  var mapjs;

  // debugger

  mapjs = window.seedysoft.maps.instances[elementId];
  if (!mapjs) {
    mapjs = IDEE.map({
      center: { x: center.x, y: center.y, draw: true },
      container: elementId,
      controls: ["backgroundlayers", "location"], // ["scale","scaleline","panzoombar","panzoom","location","getfeatureinfo","rotate","backgroundlayers","attributions","implementationswitcher"]
      maxZoom: 20,
      minZoom: 4,
      //projection: { code: 'EPSG:4326', datum: 'd', asDefault: true },
      zoom: zoom
    });

    // const layer = new IDEE.layer.WMS({
    //   url: 'https://geoportalgasolineras.es/cgi-bin/mapserv?request=GetCapabilities&service=WMS',
    //   name: 'Estaciones de servicio',
    //   legend: 'Estaciones de servicio',
    //   maxZoom: 20,
    //   minZoom: 6,
    //   visibility: true,
    //   tiled: false,
    // }, {});
    // mapjs.addWMS(layer);

    // const estacionesLayer = new IDEE.layer.KML({
    //   url: 'https://geoportalgasolineras.es/kml/estaciones',
    //   name: 'Estaciones de servicio',
    //   extract: false
    // });
    // mapjs.addLayers([estacionesLayer]);

    // console.log(IDEE.impl.ol.js.projections.getSupportedProjs());

    // mapjs = mapjs
    //   .setProjection('EPSG:4326', true)
    //   .setCenter({ x: center.x, y: center.y, draw: true })
    //   .setZoom(zoom);

    // Otras formas de añadir controles
    //mapjs.addControls(['scale', 'location', 'backgroundlayers']);
    mapjs.addControls([
      // new IDEE.control.Attributions(),
      // new IDEE.control.BackgroundLayers(),
      // new IDEE.control.GetFeatureInfo(),
      // new IDEE.control.IGNSearchLocatorControl(),
      // new IDEE.control.ImplementationSwitcher(),
      // new IDEE.control.InfoCatastroControl(),
      // new IDEE.control.LayerswitcherControl(),
      // new IDEE.control.Location(),
      // new IDEE.control.LocationControl(),
      // new IDEE.control.MouseSRSControl(),
      // new IDEE.control.Panzoom(),
      // new IDEE.control.Panzoombar(),
      // new IDEE.control.PredefinedZoomControl(),
      // new IDEE.control.Rotate(),
      new IDEE.control.Scale(),
      // new IDEE.control.ScaleLine(),
      // new IDEE.control.ShareMapControl(),
      // new IDEE.control.ViewHistoryControl(),
      // new IDEE.control.ViewManagementControl(),
      // new IDEE.control.WMCSelector(),
      // new IDEE.control.XYLocatorControl(),
      // new IDEE.control.ZoomExtentControl(),
      // new IDEE.control.ZoomPanelControl()
    ]);

    // Obtener array de controles del mapa
    const controles = mapjs.getControls();
    // for (var i = 0; i < controles.length ; i++) {
    //   controles[i].activate();
    // }

    // // Acceso y activación de un control por código:
    // const ctrlLocation = mapjs.getControls({ name: 'location' })[0];
    // ctrlLocation.activate(); //para activarlo
    // ctrlLocation.deactivate(); //para desactivarlo

    // Configuración de los plugins
    // const mp = new IDEE.plugin.Locator({
    //   position: 'BC',
    // });
    // mapjs.addPlugin(mp);

    // const mp1 = new IDEE.plugin.ShareMap({
    //   baseUrl: 'https://componentes.idee.es/api-idee/',
    //   position: 'BL',
    // });
    // // mapjs.addPlugin(mp1);

    // const mp2 = new IDEE.plugin.ViewManagement();
    // mapjs.addPlugin(mp2);

    // const mp3 = new IDEE.plugin.MouseSRS({
    //   srs: 'EPSG:4326',
    //   label: 'WGS84',
    //   precision: 6,
    //   geoDecimalDigits: 4,
    //   utmDecimalDigits: 2,
    // });
    // mapjs.addPlugin(mp3);

    const layerswitcherPlugin = new IDEE.plugin.Layerswitcher({
      collapsed: true,
      collapsible: true,
      position: 'TR',
    });
    mapjs.addPlugin(layerswitcherPlugin);

    window.seedysoft.maps.instances[elementId] = (mapjs);
  }

  return mapjs;
}

// // Crear funciones para pop up:
// //   IDEE.dialog.info('Mensaje informativo'); // color azul
// //   IDEE.dialog.error('Mensaje de error'); // color rojo
// //   IDEE.dialog.success('Mensaje de éxito'); // color verde

// // HTML en un pop up:
// // const featureTabOpts = {
// //   'icon': 'g-cartografia-pin',
// //   'title': 'Título de la pestaña',
// //   'content': 'Código html que se quiere mostrar en la pestaña'
// // };
// // // Creamos el Popup y le añadimos la pestaña
// // popup = new IDEE.Popup();
// // popup.addTab(featureTabOpts);
// // // Finalmente se añade al mapa, especificando las Coordenadas
// // mapjs.addPopup(popup, [-467062.8225, 4683459.6216]);


// // Plugin Measurebar
// // const measurebar = new IDEE.plugin.MeasureBar({
// //   postition: 'TL',
// //   collapsible: true,
// //   collapsed: true,
// // });

// // mapjs.addPlugin(measurebar);

// // // Cuando el plugin se activa, se muestra un mensaje.
// // // PARA VER EL EFECTO QUE TIENE: activar la herramienta de medición de longitud
// // // haciendo click en la primera opción del plugin measurebar
// // measurebar.controls_[0].on(IDEE.evt.ACTIVATED, () => {
// //   IDEE.dialog.info('ACTIVATED');
// // });
