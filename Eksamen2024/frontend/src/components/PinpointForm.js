import React, { useState, useEffect } from 'react';

function PinpointForm({ onCreate, onUpdate, selectedPinpoint, clearSelectedPinpoint }) {
  const [pinpointData, setPinpointData] = useState({ name: '', description: '' });

  // Oppdaterer skjemaet når et pinpoint er valgt for redigering
  useEffect(() => {
    if (selectedPinpoint) {
      setPinpointData({
        id: selectedPinpoint.pinpointId,
        name: selectedPinpoint.name,
        description: selectedPinpoint.description,
      });
    } else {
      setPinpointData({ name: '', description: '' });
    }
  }, [selectedPinpoint]);

  // Håndter skjemaendringer
  const handleChange = (e) => {
    const { name, value } = e.target;
    setPinpointData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Håndter oppretting eller oppdatering
  const handleSubmit = (e) => {
    e.preventDefault();

    if (pinpointData.id) {
      // Oppdatering
      onUpdate(pinpointData);
    } else {
      // Oppretting
      onCreate(pinpointData);
    }
  };

  return (
    <div>
      <h2>{selectedPinpoint ? 'Edit Pinpoint' : 'Create New Pinpoint'}</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          name="name"
          placeholder="Name"
          value={pinpointData.name}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="description"
          placeholder="Description"
          value={pinpointData.description}
          onChange={handleChange}
          required
        />
        <button type="submit">{selectedPinpoint ? 'Update' : 'Create'}</button>
        {selectedPinpoint && (
          <button type="button" onClick={clearSelectedPinpoint}>
            Cancel
          </button>
        )}
      </form>
    </div>
  );
}

export default PinpointForm;
