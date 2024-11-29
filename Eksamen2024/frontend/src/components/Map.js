import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { createPinpoint } from '../apiService';

const Map = ({ pinpoints, onPinpointAdded, onPinpointUpdated, onPinpointDeleted }) => {
  const mapRef = useRef(null);
  const markersRef = useRef({}); // Holder oversikt over markørene for hver pinpoint

  useEffect(() => {
    if (!mapRef.current) {
      mapRef.current = L.map('map').setView([59.91, 10.75], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors',
      }).addTo(mapRef.current);

      mapRef.current.on('click', async (e) => {
        const { lat, lng } = e.latlng;
        const popupContent = `
          <div>
            <label>Name:</label><br>
            <input type="text" id="name" placeholder="Enter name" /><br>
            <label>Description:</label><br>
            <textarea id="description" placeholder="Enter description"></textarea><br>
            <button id="createPinpoint">Create</button>
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

            addMarker(createdPinpoint); // Legg til markør for det nye pinpointet
          } catch (error) {
            console.error('Error creating pinpoint:', error);
          }
        });
      });
    }

    // Oppdater markørene på kartet
    updateMarkers();

    return () => {
      // Fjern gamle markører ved avmontering
      clearMarkers();
    };
  }, [pinpoints]);

  // Legg til en markør for et pinpoint
  const addMarker = (pinpoint) => {
    const marker = L.marker([pinpoint.latitude, pinpoint.longitude])
      .addTo(mapRef.current)
      .bindPopup(`
        <b>${pinpoint.name}</b><br>${pinpoint.description}<br>
        <button onclick="window.editPinpoint(${pinpoint.pinpointId})">Edit</button>
        <button onclick="window.deletePinpoint(${pinpoint.pinpointId})">Delete</button>
      `);

    markersRef.current[pinpoint.pinpointId] = marker;

    window.editPinpoint = (id) => {
      const selected = pinpoints.find((p) => p.pinpointId === id);
      if (selected) {
        const name = prompt('Edit name:', selected.name);
        const description = prompt('Edit description:', selected.description);
        if (name && description) {
          onPinpointUpdated({ ...selected, name, description });
        }
      }
    };

    window.deletePinpoint = (id) => {
      const confirmDelete = window.prompt(
        `Type DELETE to confirm deletion of pinpoint with ID ${id}:`
      );
      if (confirmDelete === 'DELETE') {
        onPinpointDeleted(id);
        removeMarker(id); // Fjern markøren fra kartet
      }
    };
  };

  // Fjern en markør fra kartet
  const removeMarker = (id) => {
    const marker = markersRef.current[id];
    if (marker) {
      mapRef.current.removeLayer(marker);
      delete markersRef.current[id];
    }
  };

  // Oppdater alle markører på kartet
  const updateMarkers = () => {
    clearMarkers();
    pinpoints.forEach(addMarker);
  };

  // Fjern alle markører fra kartet
  const clearMarkers = () => {
    Object.values(markersRef.current).forEach((marker) => {
      mapRef.current.removeLayer(marker);
    });
    markersRef.current = {};
  };

  return <div id="map" style={{ height: '100%', width: '100%' }}></div>;
};

export default Map;
