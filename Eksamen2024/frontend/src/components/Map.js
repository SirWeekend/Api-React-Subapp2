import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { createPinpoint } from '../apiService';

const Map = ({ pinpoints = [], onPinpointAdded }) => {
  const mapRef = useRef(null);

  useEffect(() => {
    if (!mapRef.current) {
      // Initialiser kartet
      mapRef.current = L.map('map').setView([59.91, 10.75], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
      }).addTo(mapRef.current);

      // Håndter klikk på kartet for å opprette nye pinpoints
      mapRef.current.on('click', async (e) => {
        const { lat, lng } = e.latlng;
        const popupContent = `
          <div>
            <label for="name">Name:</label><br>
            <input type="text" id="name" placeholder="Enter name" /><br>
            <label for="description">Description:</label><br>
            <textarea id="description" placeholder="Enter description"></textarea><br>
            <button id="createPinpoint">Create Pinpoint</button>
          </div>
        `;

        const popup = L.popup()
          .setLatLng(e.latlng)
          .setContent(popupContent)
          .openOn(mapRef.current);

        // Legg til eventlistener for å opprette nytt pinpoint
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
            onPinpointAdded(createdPinpoint);
            mapRef.current.closePopup();

            // Legg til markør på kartet for det nye pinpointet
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

    // Oppdater eksisterende pinpoints på kartet
    console.log('Updating map with pinpoints:', pinpoints);

    pinpoints.forEach((pinpoint) => {
      if (!pinpoint || !pinpoint.latitude || !pinpoint.longitude) {
        console.warn(`Skipping invalid pinpoint on map: ${JSON.stringify(pinpoint)}`);
        return; // Hopp over ugyldige pinpoints
      }

      console.log(`Adding pinpoint to map: ID=${pinpoint.pinpointId}, Name=${pinpoint.name}`);
      L.marker([pinpoint.latitude, pinpoint.longitude])
        .addTo(mapRef.current)
        .bindPopup(`<b>${pinpoint.name}</b><br>${pinpoint.description}`);
    });
  }, [pinpoints]);

  return <div id="map" style={{ height: '500px', width: '100%' }}></div>;
};

export default Map;
