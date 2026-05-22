/// reference path="https://componentes.idee.es/api-idee/vendor/browser-polyfill.js" />
/// reference path="https://componentes.idee.es/api-idee/js/apiidee.ol.min.js" />
/// reference path="https://componentes.idee.es/api-idee/js/configuration.js" />

window.seedysoft.maps = window.seedysoft.maps || {
  instances: []
}

/**
 * @param {string} elementId - The name of the div container for the map
 * @param {Object} options - The options for the map
 */
function createMap(elementId, options) {
  var map;

  map = window.seedysoft.maps.instances[elementId];
  if (!map) {
    // Configuración del mapa
    map = IDEE.map({
      container: elementId,

      // controls: ['panzoombar','panzoom', 'scale*true', 'scaleline', 'rotate', 'location', 'backgroundlayers'],  

      controls: ['location'],// Modificar a gusto la siguiente lista con los valores arriba indicados
      zoom: 5,
      maxZoom: 20,
      minZoom: 4,
      center: [-467062.8225, 4683459.6216],
    });

    // Otras formas de añadir controles
    //map.addControls(['scale', 'location', 'backgroundlayers']);
    //map.addControls([
    //new IDEE.control.Rotate(),
    //new IDEE.control.Panzoombar()]);

    // Obtener array de controles del mapa
    const controles = map.getControls();

    // Acceso y activación de un control por código:
    const ctrlLocation = map.getControls({ name: 'location' })[0];
    ctrlLocation.activate(); //para activarlo
    ctrlLocation.deactivate(); //para desactivarlo

    window.seedysoft.maps.instances[elementId] = (map);
  }

  return map;
}
