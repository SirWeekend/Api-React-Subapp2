import React, { useState, useEffect } from 'react';

const PinpointForm = ({ onCreate, onUpdate, selectedPinpoint, clearSelectedPinpoint }) => {
  const [pinpoint, setPinpoint] = useState({
    name: '',
    description: '',
    latitude: '',
    longitude: '',
  });

  useEffect(() => {
    if (selectedPinpoint) {
      setPinpoint(selectedPinpoint);
    } else {
      setPinpoint({ name: '', description: '', latitude: '', longitude: '' });
    }
  }, [selectedPinpoint]);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (pinpoint.pinpointId) {
      onUpdate(pinpoint);
    } else {
      onCreate(pinpoint);
    }
  };

  return (
    <div>
      <h2>{pinpoint.pinpointId ? 'Edit Pinpoint' : 'Create New Pinpoint'}</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Name"
          value={pinpoint.name}
          onChange={(e) => setPinpoint({ ...pinpoint, name: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Description"
          value={pinpoint.description}
          onChange={(e) => setPinpoint({ ...pinpoint, description: e.target.value })}
          required
        />
        <input
          type="number"
          placeholder="Latitude"
          value={pinpoint.latitude}
          onChange={(e) => setPinpoint({ ...pinpoint, latitude: parseFloat(e.target.value) })}
          required
        />
        <input
          type="number"
          placeholder="Longitude"
          value={pinpoint.longitude}
          onChange={(e) => setPinpoint({ ...pinpoint, longitude: parseFloat(e.target.value) })}
          required
        />
        <button type="submit">{pinpoint.pinpointId ? 'Update' : 'Create'}</button>
        {pinpoint.pinpointId && <button onClick={clearSelectedPinpoint}>Cancel</button>}
      </form>
    </div>
  );
};

export default PinpointForm;
