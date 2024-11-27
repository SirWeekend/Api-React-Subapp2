import React from 'react';

const PinpointList = ({ pinpoints, onDelete, onEdit }) => {
  return (
    <div>
      <h2>Pinpoints</h2>
      <ul>
        {pinpoints.map((pinpoint) => (
          <li key={pinpoint.pinpointId}>
            <strong>{pinpoint.name}</strong>: {pinpoint.description}
            <button onClick={() => onDelete(pinpoint.pinpointId)}>Delete</button>
            <button onClick={() => onEdit(pinpoint)}>Edit</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default PinpointList;
