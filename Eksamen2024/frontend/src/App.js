import React, { useState, useEffect } from 'react';
import PinpointList from './components/PinpointList';
import PinpointForm from './components/PinpointForm';
import Map from './components/Map'; // Legg til Map-komponenten
import Login from './components/Login'; // Login-komponenten
import Register from './components/Register'; // Register-komponenten
import { logoutUser } from './apiService'; // Funksjon for å logge ut
import {
  fetchPinpoints,
  createPinpoint as apiCreatePinpoint,
  updatePinpoint as apiUpdatePinpoint,
  deletePinpoint as apiDeletePinpoint,
} from './apiService';

function App() {
  const [pinpoints, setPinpoints] = useState([]); // Lagrer data fra API
  const [selectedPinpoint, setSelectedPinpoint] = useState(null); // For oppdatering (PUT)
  const [user, setUser] = useState(null); // Lagrer innlogget bruker

  // Hente data fra backend (GET)
  const loadPinpoints = async () => {
    try {
      const data = await fetchPinpoints();
      setPinpoints(data); // Oppdater state
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };

  // Opprette en ny pinpoint (POST)
  const handleCreate = async (newPinpoint) => {
    try {
      const createdPinpoint = await apiCreatePinpoint(newPinpoint);
      setPinpoints([...pinpoints, createdPinpoint]); // Oppdater state med ny pinpoint
    } catch (error) {
      console.error('Error creating pinpoint:', error);
    }
  };

  // Oppdatere en eksisterende pinpoint (PUT)
  const handleUpdate = async (updatedPinpoint) => {
    try {
      const { id, name, description } = updatedPinpoint;
      await apiUpdatePinpoint(id, { name, description });
      setPinpoints(
        pinpoints.map((pinpoint) =>
          pinpoint.pinpointId === id ? { ...pinpoint, name, description } : pinpoint
        )
      );
      setSelectedPinpoint(null); // Nullstill valgt pinpoint
    } catch (error) {
      console.error('Error updating pinpoint:', error);
    }
  };

  // Slette en pinpoint (DELETE)
  const handleDelete = async (id) => {
    try {
      await apiDeletePinpoint(id);
      setPinpoints(pinpoints.filter((pinpoint) => pinpoint.pinpointId !== id)); // Fjern pinpoint fra state
    } catch (error) {
      console.error('Error deleting pinpoint:', error);
    }
  };

  // Hente data ved lasting av siden
  useEffect(() => {
    if (user) {
      loadPinpoints();
    }
  }, [user]);

  // Logge ut brukeren
  const handleLogout = async () => {
    await logoutUser();
    setUser(null); // Nullstill brukerstatus
  };

  return (
    <div>
      <h1>Pinpoints App</h1>
      {!user ? (
        <>
          <Login setUser={setUser} /> {/* Login-komponenten */}
          <Register /> {/* Register-komponenten */}
        </>
      ) : (
        <>
          <p>Welcome, {user.username}!</p>
          <button onClick={handleLogout}>Logout</button>
          <Map pinpoints={pinpoints} /> {/* Kartkomponenten */}
          <PinpointList
            pinpoints={pinpoints}
            onDelete={handleDelete}
            onEdit={setSelectedPinpoint} // Setter valgt pinpoint for redigering
          />
          <PinpointForm
            onCreate={handleCreate}
            onUpdate={handleUpdate}
            selectedPinpoint={selectedPinpoint}
            clearSelectedPinpoint={() => setSelectedPinpoint(null)}
          />
        </>
      )}
    </div>
  );
}

export default App;
