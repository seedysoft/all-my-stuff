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

/**
 * @param {string} elementId  - The name of the div container for the mapjs
 * @param {LngLat} center     - The LngLat where mapjs starts
 * @param {double} zoom       - The zoom level for the mapjs
 * @return {IDEE.Map}         - The mapjs instance
 */
export function createMap(elementId, center, zoom) {
  var mapjs;

  mapjs = window.seedysoft.maps.instances[elementId];
  if (!mapjs) {
    // Configuración del mapa
    mapjs = IDEE.map({
      container: elementId,

      // controls: ['panzoombar','panzoom', 'scale*true', 'scaleline', 'rotate', 'location', 'backgroundlayers'],

      // controls: ['panzoombar', 'panzoom', 'scale*true', 'scaleline', 'location'],
      controls: ['panzoom', 'scale*true'],
      zoom: zoom,
      maxZoom: 20,
      minZoom: 4,
      center: center,
    });

    // Otras formas de añadir controles
    //mapjs.addControls(['scale', 'location', 'backgroundlayers']);
    //mapjs.addControls([
    //new IDEE.control.Rotate(),
    //new IDEE.control.Panzoombar()]);

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
    const mp = new IDEE.plugin.Locator({
      position: 'BC',
    });
    // const mp1 = new IDEE.plugin.ShareMap({
    //   baseUrl: 'https://componentes.idee.es/api-idee/',
    //   position: 'BL',
    // });
    const mp2 = new IDEE.plugin.ViewManagement();
    const mp3 = new IDEE.plugin.MouseSRS({
      srs: 'EPSG:4326',
      label: 'WGS84',
      precision: 6,
      geoDecimalDigits: 4,
      utmDecimalDigits: 2,
    });
    const mp4 = new IDEE.plugin.Layerswitcher({
      collapsed: true,
      collapsible: true,
      position: 'TR',
    });

    // mapjs.addPlugin(mp);
    // // mapjs.addPlugin(mp1);
    // mapjs.addPlugin(mp2);
     mapjs.addPlugin(mp3);
    // mapjs.addPlugin(mp4);

    window.seedysoft.maps.instances[elementId] = (mapjs);
  }

  return mapjs;
}

// Crear funciones para pop up:
//   IDEE.dialog.info('Mensaje informativo'); // color azul
//   IDEE.dialog.error('Mensaje de error'); // color rojo
//   IDEE.dialog.success('Mensaje de éxito'); // color verde

// HTML en un pop up:
// const featureTabOpts = {
//   'icon': 'g-cartografia-pin',
//   'title': 'Título de la pestaña',
//   'content': 'Código html que se quiere mostrar en la pestaña'
// };
// // Creamos el Popup y le añadimos la pestaña
// popup = new IDEE.Popup();
// popup.addTab(featureTabOpts);
// // Finalmente se añade al mapa, especificando las Coordenadas
// mapjs.addPopup(popup, [-467062.8225, 4683459.6216]);


// Plugin Measurebar
// const measurebar = new IDEE.plugin.MeasureBar({
//   postition: 'TL',
//   collapsible: true,
//   collapsed: true,
// });

// mapjs.addPlugin(measurebar);

// // Cuando el plugin se activa, se muestra un mensaje.
// // PARA VER EL EFECTO QUE TIENE: activar la herramienta de medición de longitud
// // haciendo click en la primera opción del plugin measurebar 
// measurebar.controls_[0].on(IDEE.evt.ACTIVATED, () => {
//   IDEE.dialog.info('ACTIVATED');
// });
