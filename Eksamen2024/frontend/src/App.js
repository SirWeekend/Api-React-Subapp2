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
  const [pinpoints, setPinpoints] = useState([]); // State for pinpoints
  const [selectedPinpoint, setSelectedPinpoint] = useState(null); // State for valgt pinpoint
  const [user, setUser] = useState(null); // State for brukerinformasjon

  const loadPinpoints = async () => {
    try {
      const data = await fetchPinpoints();
      console.log('Pinpoints fetched from backend:', data);
      setPinpoints(data);
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };

  const handlePinpointAdded = (newPinpoint) => {
    setPinpoints((prevPinpoints) => [...prevPinpoints, newPinpoint]);
  };

  const handlePinpointUpdated = (updatedPinpoint) => {
    setPinpoints((prev) =>
      prev.map((p) =>
        p.pinpointId === updatedPinpoint.pinpointId ? updatedPinpoint : p
      )
    );
    console.log('Pinpoint updated in state:', updatedPinpoint);
  };

  const handlePinpointDeleted = (id) => {
    setPinpoints((prevPinpoints) =>
      prevPinpoints.filter((p) => p.pinpointId !== id)
    );
    console.log('Pinpoint deleted from state with ID:', id);
  };

  const handleUpdate = async (updatedPinpoint) => {
    try {
      console.log('Sending data to update pinpoint:', updatedPinpoint);
      await updatePinpoint(updatedPinpoint.pinpointId, updatedPinpoint);
      handlePinpointUpdated(updatedPinpoint);
      setSelectedPinpoint(null);
    } catch (error) {
      console.error('Error updating pinpoint:', error.response?.data || error.message);
    }
  };

  const handleDelete = async (id) => {
    try {
      await deletePinpoint(id);
      handlePinpointDeleted(id);
    } catch (error) {
      console.error('Error deleting pinpoint:', error);
    }
  };

  useEffect(() => {
    if (user) {
      console.log('User logged in:', user);
      loadPinpoints(); // Last pinpoints når brukeren logger inn
    }
  }, [user]);

  const handleLogout = async () => {
    try {
      await logoutUser();
      console.log('User logged out');
      setUser(null); // Nullstill brukerdata
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
          <Map
            pinpoints={pinpoints}
            onPinpointAdded={handlePinpointAdded}
            onPinpointUpdated={handlePinpointUpdated}
            onPinpointDeleted={handlePinpointDeleted}
          />
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
