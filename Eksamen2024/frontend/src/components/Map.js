import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { createPinpoint, updatePinpoint, deletePinpoint } from '../apiService';

const Map = ({ pinpoints = [], onPinpointAdded, onPinpointUpdated, onPinpointDeleted }) => {
  const mapRef = useRef(null);

  // Definer det tilpassede ikonet
  const customIcon = L.icon({
    iconUrl: '/custom-marker-icon.png', // Banen til bildet i public-mappen
    iconSize: [30, 42], // Tilpasset størrelse
    iconAnchor: [15, 42],
    popupAnchor: [0, -40],
  });
  

  useEffect(() => {
    if (!mapRef.current) {
      // Initialiser kartet
      mapRef.current = L.map('map').setView([59.91, 10.75], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
      }).addTo(mapRef.current);

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

            L.marker([createdPinpoint.latitude, createdPinpoint.longitude], { icon: customIcon })
              .addTo(mapRef.current)
              .bindPopup(`<b>${createdPinpoint.name}</b><br>${createdPinpoint.description}`);
          } catch (error) {
            console.error('Error creating pinpoint:', error);
            alert('Failed to create pinpoint.');
          }
        });
      });
    }

    // Fjern eksisterende markører
    mapRef.current.eachLayer((layer) => {
      if (layer instanceof L.Marker) {
        mapRef.current.removeLayer(layer);
      }
    });

    // Legg til markører for hver pinpoint
    pinpoints.forEach((pinpoint) => {
      if (pinpoint.latitude && pinpoint.longitude) {
        L.marker([pinpoint.latitude, pinpoint.longitude], { icon: customIcon })
          .addTo(mapRef.current)
          .bindPopup(`
            <b>${pinpoint.name}</b><br>${pinpoint.description || 'No description available.'}<br>
            <button onclick="window.handleEdit(${pinpoint.pinpointId})">Edit</button>
            <button onclick="window.handleDelete(${pinpoint.pinpointId})" style="color: red;">Delete</button>
          `);
      }
    });

    // Expose edit and delete handlers globally for popup buttons
    window.handleEdit = (id) => {
      const pinpoint = pinpoints.find((p) => p.pinpointId === id);
      if (pinpoint) {
        const newName = prompt('Enter new name:', pinpoint.name);
        const newDescription = prompt('Enter new description:', pinpoint.description);
        if (newName && newDescription) {
          onPinpointUpdated({ ...pinpoint, name: newName, description: newDescription });
        }
      }
    };

    window.handleDelete = (id) => {
      if (window.confirm('Are you sure you want to delete this pinpoint?')) {
        onPinpointDeleted(id);
      }
    };
  }, [pinpoints]);

  return <div id="map" style={{ height: '100vh', width: '100%' }}></div>;
};

export default Map;
