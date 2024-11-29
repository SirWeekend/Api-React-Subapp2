import React from 'react';

const PinpointList = ({ pinpoints = [], onDelete, onEdit }) => {
  console.log('Rendering PinpointList with pinpoints:', pinpoints);

  return (
    <div>
      <h2>Pinpoints</h2>
      {pinpoints.length > 0 ? (
        <ul>
          {pinpoints.map((pinpoint) => {
            if (!pinpoint || !pinpoint.pinpointId || !pinpoint.name) {
              console.warn('Skipping invalid pinpoint:', pinpoint);
              return null; // Hopp over ugyldige pinpoints
            }

            console.log(`Rendering pinpoint with ID: ${pinpoint.pinpointId}, Name: ${pinpoint.name}`);
            return (
              <li key={pinpoint.pinpointId}>
                <strong>{pinpoint.name}</strong>: {pinpoint.description}
                <button onClick={() => onDelete(pinpoint.pinpointId)}>Delete</button>
                <button onClick={() => onEdit(pinpoint)}>Edit</button>
              </li>
            );
          })}
        </ul>
      ) : (
        <p>No pinpoints available.</p>
      )}
    </div>
  );
};

export default PinpointList;

