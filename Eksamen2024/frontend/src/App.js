import React, { useState, useEffect } from 'react';
import PinpointList from './components/PinpointList';
import PinpointForm from './components/PinpointForm';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import {
  fetchPinpoints,
  createPinpoint,
  updatePinpoint,
  deletePinpoint,
  logoutUser,
} from './apiService';

function App() {
  const [pinpoints, setPinpoints] = useState([]);
  const [selectedPinpoint, setSelectedPinpoint] = useState(null);
  const [user, setUser] = useState(null);

  const loadPinpoints = async () => {
    try {
      const data = await fetchPinpoints();
      console.log('Fetched pinpoints:', data);
      setPinpoints(data);
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };

  const handlePinpointAdded = async (newPinpoint) => {
    try {
      const createdPinpoint = await createPinpoint(newPinpoint);
      console.log('Pinpoint added from map:', createdPinpoint);
      setPinpoints((prevPinpoints) => [...prevPinpoints, createdPinpoint]);
    } catch (error) {
      console.error('Error adding pinpoint:', error);
    }
  };

  const handleUpdate = async (updatedPinpoint) => {
    try {
      await updatePinpoint(updatedPinpoint.pinpointId, updatedPinpoint);
      console.log('Pinpoint updated:', updatedPinpoint);
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
      await deletePinpoint(id);
      console.log('Pinpoint deleted with ID:', id);
      setPinpoints((prevPinpoints) => prevPinpoints.filter((p) => p.pinpointId !== id));
    } catch (error) {
      console.error('Error deleting pinpoint:', error);
    }
  };

  useEffect(() => {
    if (user) {
      console.log('User logged in:', user);
      loadPinpoints();
    }
  }, [user]);

  const handleLogout = async () => {
    try {
      await logoutUser();
      console.log('User logged out');
      setUser(null);
    } catch (error) {
      console.error('Error during logout:', error);
    }
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
            onCreate={handlePinpointAdded}
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
