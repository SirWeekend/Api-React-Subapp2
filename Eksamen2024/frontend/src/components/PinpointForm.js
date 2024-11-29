import React, { useState, useEffect } from 'react';

const PinpointForm = ({ onCreate, onUpdate, selectedPinpoint, clearSelectedPinpoint }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [latitude, setLatitude] = useState('');
  const [longitude, setLongitude] = useState('');

  useEffect(() => {
    if (selectedPinpoint) {
      setName(selectedPinpoint.name || '');
      setDescription(selectedPinpoint.description || '');
      setLatitude(selectedPinpoint.latitude || '');
      setLongitude(selectedPinpoint.longitude || '');
    } else {
      setName('');
      setDescription('');
      setLatitude('');
      setLongitude('');
    }
  }, [selectedPinpoint]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const pinpointData = { name, description, latitude, longitude };

    if (selectedPinpoint) {
      pinpointData.pinpointId = selectedPinpoint.pinpointId;
      await onUpdate(pinpointData);
    } else {
      await onCreate(pinpointData);
    }

    clearSelectedPinpoint();
  };

  return (
    <div>
      <h2>{selectedPinpoint ? 'Edit Pinpoint' : 'Create New Pinpoint'}</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <textarea
          placeholder="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          required
        ></textarea>
        <input
          type="number"
          placeholder="Latitude"
          value={latitude}
          onChange={(e) => setLatitude(e.target.value)}
          required
        />
        <input
          type="number"
          placeholder="Longitude"
          value={longitude}
          onChange={(e) => setLongitude(e.target.value)}
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
};

export default PinpointForm;
