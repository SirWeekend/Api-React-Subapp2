import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { createPinpoint } from '../apiService';

const Map = ({ pinpoints, onPinpointAdded }) => {
  const mapRef = useRef(null);

  useEffect(() => {
    if (!mapRef.current) {
      // Initialiser kartet bare én gang
      mapRef.current = L.map('map').setView([59.91, 10.75], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
      }).addTo(mapRef.current);

      // Event listener for å legge til pinpoint ved klikk
      mapRef.current.on('click', async (e) => {
        const { lat, lng } = e.latlng;

        // Popup-skjema
        const popup = L.popup()
          .setLatLng(e.latlng)
          .setContent(`
            <div>
              <label for="name">Name:</label><br>
              <input type="text" id="name" placeholder="Enter name" /><br>
              <label for="description">Description:</label><br>
              <textarea id="description" placeholder="Enter description"></textarea><br>
              <button id="createPinpoint">Create Pinpoint</button>
            </div>
          `)
          .openOn(mapRef.current);

        document.getElementById('createPinpoint').addEventListener('click', async () => {
          const name = document.getElementById('name').value;
          const description = document.getElementById('description').value;

          if (!name || !description) {
            alert('Name and description are required!');
            return;
          }

          const newPinpoint = { name, description, latitude: lat, longitude: lng };

          try {
            const createdPinpoint = await createPinpoint(newPinpoint);
            onPinpointAdded(createdPinpoint); // Oppdater state i App.js
            mapRef.current.closePopup();

            // Legg til markør for det nye pinpointet
            L.marker([createdPinpoint.latitude, createdPinpoint.longitude])
              .addTo(mapRef.current)
              .bindPopup(`<b>${createdPinpoint.name}</b><br>${createdPinpoint.description}`);
          } catch (error) {
            console.error('Error creating pinpoint:', error);
            alert('Failed to create pinpoint.');
          }
        });
      });
    }

    // Fjern gamle markører
    mapRef.current.eachLayer((layer) => {
      if (layer instanceof L.Marker) {
        mapRef.current.removeLayer(layer);
      }
    });

    // Legg til eksisterende pinpoints
    pinpoints.forEach((pinpoint) => {
      if (pinpoint.latitude && pinpoint.longitude) {
        L.marker([pinpoint.latitude, pinpoint.longitude])
          .addTo(mapRef.current)
          .bindPopup(`<b>${pinpoint.name}</b><br>${pinpoint.description}`);
      }
    });
  }, [pinpoints, onPinpointAdded]);

  return <div id="map" style={{ height: '500px', width: '100%' }}></div>;
};

export default Map;
