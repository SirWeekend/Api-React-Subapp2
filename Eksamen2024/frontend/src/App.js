import React, { useState, useEffect } from 'react';
import Map from './components/Map';
import Login from './components/Login';
import Register from './components/Register';
import { fetchPinpoints, createPinpoint, updatePinpoint, deletePinpoint, logoutUser } from './apiService';

function App() {
  const [pinpoints, setPinpoints] = useState([]);
  const [user, setUser] = useState(null);

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

  const handleUpdate = async (updatedPinpoint) => {
    try {
      console.log('Updating pinpoint:', updatedPinpoint);
      await updatePinpoint(updatedPinpoint.pinpointId, updatedPinpoint);
      setPinpoints((prev) =>
        prev.map((p) =>
          p.pinpointId === updatedPinpoint.pinpointId ? updatedPinpoint : p
        )
      );
    } catch (error) {
      console.error('Error updating pinpoint:', error.response?.data || error.message);
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
    <div style={{ display: 'flex', height: '100vh' }}>
      <div style={{ width: '250px', backgroundColor: '#f4f4f4', padding: '10px' }}>
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
          </>
        )}
      </div>
      <div style={{ flex: 1 }}>
        {user && (
          <Map
            pinpoints={pinpoints}
            onPinpointAdded={handlePinpointAdded}
            onPinpointUpdated={handleUpdate}
            onPinpointDeleted={handleDelete}
          />
        )}
      </div>
    </div>
  );
}

export default App;
