import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { createPinpoint, updatePinpoint, deletePinpoint } from '../apiService';

const Map = ({ pinpoints = [], onPinpointAdded, onPinpointUpdated, onPinpointDeleted }) => {
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

            addPinpointMarker(createdPinpoint); // Legg til ny markør
          } catch (error) {
            console.error('Error creating pinpoint:', error);
            alert('Failed to create pinpoint.');
          }
        });
      });
    }

    // Fjern eksisterende markører for å oppdatere dem
    mapRef.current.eachLayer((layer) => {
      if (layer instanceof L.Marker) {
        layer.remove();
      }
    });

    // Legg til markører for eksisterende pinpoints
    pinpoints.forEach(addPinpointMarker);
  }, [pinpoints]);

  // Funksjon for å legge til markører med knapper
  const addPinpointMarker = (pinpoint) => {
    const marker = L.marker([pinpoint.latitude, pinpoint.longitude]).addTo(mapRef.current);

    const popupContent = `
      <div>
        <strong>${pinpoint.name}</strong><br>
        ${pinpoint.description}<br><br>
        <button id="edit-${pinpoint.pinpointId}" class="popup-btn">Edit Pinpoint</button>
        <button id="delete-${pinpoint.pinpointId}" class="popup-btn">Delete Pinpoint</button>
      </div>
    `;

    marker.bindPopup(popupContent);

    // Når pop-up åpnes, legg til event listeners
    marker.on('popupopen', () => {
      const editButton = document.getElementById(`edit-${pinpoint.pinpointId}`);
      const deleteButton = document.getElementById(`delete-${pinpoint.pinpointId}`);

      if (editButton) {
        editButton.addEventListener('click', () => {
          const updatedName = prompt('Enter new name:', pinpoint.name);
          const updatedDescription = prompt('Enter new description:', pinpoint.description);

          if (updatedName && updatedDescription) {
            const updatedPinpoint = {
              ...pinpoint,
              name: updatedName,
              description: updatedDescription,
            };

            updatePinpoint(pinpoint.pinpointId, updatedPinpoint)
              .then(() => {
                onPinpointUpdated(updatedPinpoint);
                marker.closePopup();
              })
              .catch((err) => {
                console.error('Error updating pinpoint:', err);
                alert('Failed to update pinpoint.');
              });
          }
        });
      }

      if (deleteButton) {
        deleteButton.addEventListener('click', () => {
          if (window.confirm('Are you sure you want to delete this pinpoint?')) {
            deletePinpoint(pinpoint.pinpointId)
              .then(() => {
                onPinpointDeleted(pinpoint.pinpointId);
                marker.remove();
              })
              .catch((err) => {
                console.error('Error deleting pinpoint:', err);
                alert('Failed to delete pinpoint.');
              });
          }
        });
      }
    });
  };

  return <div id="map" style={{ height: '500px', width: '100%' }}></div>;
};

export default Map;
