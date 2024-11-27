import React, { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [pinpoints, setPinpoints] = useState([]); // Lagrer data fra API
  const [newPinpoint, setNewPinpoint] = useState({ name: '', description: '' }); // For POST
  const [updatedPinpoint, setUpdatedPinpoint] = useState({ id: null, name: '', description: '' }); // For PUT

  const apiBaseUrl = 'http://localhost:5091/api/pinpoint';

  // Hente data fra backend (GET)
  const fetchPinpoints = async () => {
    try {
      const response = await axios.get(apiBaseUrl);
      const data = response.data?.$values || [];
      setPinpoints(data); // Oppdater state med data
    } catch (error) {
      console.error('Error fetching pinpoints:', error);
    }
  };

  // Opprette en ny pinpoint (POST)
  const createPinpoint = async () => {
    try {
      console.log('Creating new pinpoint:', newPinpoint);
      const response = await axios.post(apiBaseUrl, newPinpoint);
      console.log('Response from POST:', response.data);
      setPinpoints([...pinpoints, response.data]); // Legg til ny pinpoint i state
      setNewPinpoint({ name: '', description: '' }); // Tøm skjema
    } catch (error) {
      console.error('Error creating pinpoint:', error.response ? error.response.data : error.message);
    }
  };
  

  // Oppdatere en pinpoint (PUT)
  const updatePinpoint = async () => {
    try {
      const { id, name, description } = updatedPinpoint;
      console.log('Updating pinpoint:', { id, name, description });
      const response = await axios.put(`${apiBaseUrl}/${id}`, { name, description });
      console.log('Response from PUT:', response.data);
      setPinpoints(
        pinpoints.map((pinpoint) =>
          pinpoint.pinpointId === id ? { ...pinpoint, name, description } : pinpoint
        )
      );
      setUpdatedPinpoint({ id: null, name: '', description: '' }); // Nullstill skjema
    } catch (error) {
      console.error('Error updating pinpoint:', error.response ? error.response.data : error.message);
    }
  };
  

  // Slette en pinpoint (DELETE)
  const deletePinpoint = async (id) => {
    try {
      await axios.delete(`${apiBaseUrl}/${id}`);
      setPinpoints(pinpoints.filter((pinpoint) => pinpoint.pinpointId !== id)); // Fjern fra state
    } catch (error) {
      console.error('Error deleting pinpoint:', error);
    }
  };

  // Hente data ved lasting av siden
  useEffect(() => {
    fetchPinpoints();
  }, []);

  return (
    <div>
      <h1>Pinpoints</h1>
      <ul>
        {pinpoints.map((pinpoint) => (
          <li key={pinpoint.pinpointId}>
            <strong>{pinpoint.name}</strong>: {pinpoint.description}
            <button onClick={() => deletePinpoint(pinpoint.pinpointId)}>Delete</button>
            <button
              onClick={() =>
                setUpdatedPinpoint({
                  id: pinpoint.pinpointId,
                  name: pinpoint.name,
                  description: pinpoint.description,
                })
              }
            >
              Edit
            </button>
          </li>
        ))}
      </ul>

      <h2>Create New Pinpoint</h2>
      <input
        type="text"
        placeholder="Name"
        value={newPinpoint.name}
        onChange={(e) => setNewPinpoint({ ...newPinpoint, name: e.target.value })}
      />
      <input
        type="text"
        placeholder="Description"
        value={newPinpoint.description}
        onChange={(e) => setNewPinpoint({ ...newPinpoint, description: e.target.value })}
      />
      <button onClick={createPinpoint}>Create</button>

      {updatedPinpoint.id && (
        <div>
          <h2>Edit Pinpoint</h2>
          <input
            type="text"
            placeholder="Name"
            value={updatedPinpoint.name}
            onChange={(e) =>
              setUpdatedPinpoint({ ...updatedPinpoint, name: e.target.value })
            }
          />
          <input
            type="text"
            placeholder="Description"
            value={updatedPinpoint.description}
            onChange={(e) =>
              setUpdatedPinpoint({ ...updatedPinpoint, description: e.target.value })
            }
          />
          <button onClick={updatePinpoint}>Update</button>
        </div>
      )}
    </div>
  );
}

export default App;
