import React, { useEffect } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

const Map = ({ pinpoints }) => {
  useEffect(() => {
    const map = L.map('map').setView([59.91, 10.75], 13); // Oslo som standardvisning

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(map);

    // Legg til markører basert på `pinpoints`
    pinpoints.forEach((pinpoint) => {
      L.marker([pinpoint.latitude, pinpoint.longitude])
        .addTo(map)
        .bindPopup(`<b>${pinpoint.name}</b><br>${pinpoint.description}`);
    });

    // Rydd opp kartet når komponenten fjernes
    return () => map.remove();
  }, [pinpoints]);

  return <div id="map" style={{ height: '500px', width: '100%' }}></div>;
};

export default Map;
