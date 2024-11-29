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
  
      // Sjekk dataene før de settes i state
      setPinpoints(data);
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };
  

  const handlePinpointAdded = (newPinpoint) => {
    setPinpoints((prevPinpoints) => [...prevPinpoints, newPinpoint]);
  };

  const handleUpdate = async (updatedPinpoint) => {
    try {
      await updatePinpoint(updatedPinpoint.pinpointId, updatedPinpoint);
      console.log('Pinpoint updated:', updatedPinpoint);

      setPinpoints((prev) =>
        prev.map((p) =>
          p.pinpointId === updatedPinpoint.pinpointId ? updatedPinpoint : p
        )
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

      setPinpoints((prevPinpoints) =>
        prevPinpoints.filter((p) => p.pinpointId !== id)
      );
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
