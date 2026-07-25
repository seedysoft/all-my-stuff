import { Control, DomUtil } from '../lib/leaflet/leaflet.js';

export class LoaderControl extends Control {

    constructor() {
        super();
    }

    onAdd(map) {
        this._map = map;
        this._container = L.DomUtil.create('div', 'leaflet-control-loader');
        this.hide();
        return this._container;
    }

    addTo(map) {
        this._container = this.onAdd(map);
        map.getContainer().appendChild(this._container);
        return this;
    }

    show() {
        this._container.style.display = 'block';
        return this;
    }

    hide() {
        this._container.style.display = 'none';
        return this;
    }    

};
