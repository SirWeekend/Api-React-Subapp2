import React, { useState, useEffect } from 'react';
import PinpointList from './components/PinpointList';
import PinpointForm from './components/PinpointForm';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import { logoutUser } from './apiService';
import {
  fetchPinpoints,
  createPinpoint as apiCreatePinpoint,
  updatePinpoint as apiUpdatePinpoint,
  deletePinpoint as apiDeletePinpoint,
  addComment,
  fetchComments,
} from './apiService';

function App() {
  const [pinpoints, setPinpoints] = useState([]);
  const [selectedPinpoint, setSelectedPinpoint] = useState(null);
  const [user, setUser] = useState(null);

  const loadPinpoints = async () => {
    try {
      const data = await fetchPinpoints();
      console.log('Fetched pinpoints:', data); // Logging for pinpoint-henting
      setPinpoints(data);
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };

  const handleCreate = async (newPinpoint) => {
    try {
      const createdPinpoint = await apiCreatePinpoint(newPinpoint);
      console.log('Pinpoint created:', createdPinpoint); // Logging for oppretting
      setPinpoints([...pinpoints, createdPinpoint]);
    } catch (error) {
      console.error('Error creating pinpoint:', error);
    }
  };

  const handleUpdate = async (updatedPinpoint) => {
    try {
      await apiUpdatePinpoint(updatedPinpoint.pinpointId, updatedPinpoint);
      console.log('Pinpoint updated:', updatedPinpoint); // Logging for oppdatering
      setPinpoints((prev) =>
        prev.map((p) => (p.pinpointId === updatedPinpoint.pinpointId ? updatedPinpoint : p))
      );
      setSelectedPinpoint(null);
    } catch (error) {
      console.error('Error updating pinpoint:', error);
    }
  };

  const handleDelete = async (id) => {
    try {
      await apiDeletePinpoint(id);
      console.log('Pinpoint deleted with ID:', id); // Logging for sletting
      setPinpoints(pinpoints.filter((pinpoint) => pinpoint.pinpointId !== id));
    } catch (error) {
      console.error('Error deleting pinpoint:', error);
    }
  };

  const handlePinpointAdded = (newPinpoint) => {
    console.log('Pinpoint added from map:', newPinpoint); // Logging for kart-oppretting
    setPinpoints((prevPinpoints) => [...prevPinpoints, newPinpoint]);
  };

  useEffect(() => {
    if (user) {
      console.log('User logged in:', user); // Logging for innlogging
      loadPinpoints();
    }
  }, [user]);

  const handleLogout = async () => {
    await logoutUser();
    console.log('User logged out'); // Logging for utlogging
    setUser(null);
  };

  return (
    <div>
      <h1>Pinpoints App</h1>
      {!user ? (
        <>
          <Login setUser={setUser} />
          <Register />
        </>
      ) : (
        <>
          <p>Welcome, {user.username}!</p>
          <button onClick={handleLogout}>Logout</button>
          <Map pinpoints={pinpoints} onPinpointAdded={handlePinpointAdded} />
          <PinpointList
            pinpoints={pinpoints}
            onDelete={handleDelete}
            onEdit={setSelectedPinpoint}
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
