import axios from 'axios';

const API_BASE_URL = 'http://localhost:5091/api/pinpoint';

// Login bruker
export const loginUser = async (username, password) => {
    const response = await axios.post(`${API_BASE_URL}/Bruker/Login`, { username, password });
    return response.data;
  };
  
  // Registrer ny bruker
  export const registerUser = async (username, email, password) => {
    const response = await axios.post(`${API_BASE_URL}/Bruker/Registrer`, { username, email, password });
    return response.data;
  };
  
  // Logout bruker
  export const logoutUser = async () => {
    await axios.post(`${API_BASE_URL}/Bruker/Logout`);
  };

// Henter alle pinpoints
export const fetchPinpoints = async () => {
  const response = await axios.get(API_BASE_URL);
  return response.data; // Hvis det finnes et $values, legg til .values
};

// Oppretter en ny pinpoint
export const createPinpoint = async (pinpoint) => {
  const response = await axios.post(API_BASE_URL, pinpoint);
  return response.data;
};

// Oppdaterer en eksisterende pinpoint
export const updatePinpoint = async (id, pinpoint) => {
  const response = await axios.put(`${API_BASE_URL}/${id}`, pinpoint);
  return response.data;
};

// Sletter en pinpoint
export const deletePinpoint = async (id) => {
  await axios.delete(`${API_BASE_URL}/${id}`);
};

// Henter kommentarer for en pinpoint
export const fetchComments = async (pinpointId) => {
  const response = await axios.get(`${API_BASE_URL}/${pinpointId}/comments`);
  return response.data;
};

// Legger til en kommentar
export const addComment = async (pinpointId, commentText) => {
  const response = await axios.post(`${API_BASE_URL}/${pinpointId}/comments`, { text: commentText });
  return response.data;
};
