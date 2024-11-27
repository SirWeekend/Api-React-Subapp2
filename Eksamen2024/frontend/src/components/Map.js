import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

const Map = ({ pinpoints }) => {
  const mapRef = useRef(null);

  useEffect(() => {
    if (!mapRef.current) {
      // Initialiser kartet bare én gang
      mapRef.current = L.map('map').setView([59.91, 10.75], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
      }).addTo(mapRef.current);
    }

    // Fjern tidligere markører
    mapRef.current.eachLayer((layer) => {
      if (layer instanceof L.Marker) {
        mapRef.current.removeLayer(layer);
      }
    });

    // Legg til nye markører
    pinpoints.forEach((pinpoint) => {
      if (pinpoint.latitude && pinpoint.longitude) {
        L.marker([pinpoint.latitude, pinpoint.longitude])
          .addTo(mapRef.current)
          .bindPopup(`<b>${pinpoint.name}</b><br>${pinpoint.description}`);
      }
    });
  }, [pinpoints]);

  return <div id="map" style={{ height: '500px', width: '100%' }}></div>;
};

export default Map;
