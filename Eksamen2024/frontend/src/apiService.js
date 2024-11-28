import axios from 'axios';

// Oppdatert base-URL
const API_BASE_URL = 'http://localhost:5091/api';

// Funksjon for å hente token fra localStorage
const getToken = () => localStorage.getItem('token');

// Konfigurer Axios til å inkludere Authorization-header og `withCredentials`
const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // Sørger for at cookies og cred blir sendt
});

// Legger til Authorization-header i alle forespørsler
axiosInstance.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    console.error('Error with request interceptor:', error); // Logging for feil i forespørsel
    return Promise.reject(error);
  }
);

// API-kall
export const loginUser = async (username, password) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/Bruker/Login`, { username, password });
    const token = response.data.token; // Forutsetter at serveren returnerer et token
    if (token) {
      localStorage.setItem('token', token); // Lagre token i localStorage
    }
    console.log('Login successful:', response.data); // Logging for vellykket innlogging
    return response.data;
  } catch (error) {
    console.error('Error during login:', error.response?.data || error.message); // Logging for feil ved innlogging
    throw error;
  }
};

export const registerUser = async (username, email, password) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/Bruker/Registrer`, { username, email, password });
    console.log('User registered successfully:', response.data); // Logging for vellykket registrering
    return response.data;
  } catch (error) {
    console.error('Error during registration:', error.response?.data || error.message); // Logging for feil ved registrering
    throw error;
  }
};

export const logoutUser = async () => {
  try {
    await axios.post(`${API_BASE_URL}/Bruker/Logout`);
    localStorage.removeItem('token'); // Fjern token ved utlogging
    console.log('User logged out successfully'); // Logging for utlogging
  } catch (error) {
    console.error('Error during logout:', error.response?.data || error.message); // Logging for feil ved utlogging
    throw error;
  }
};

export const fetchPinpoints = async () => {
  try {
    const response = await axiosInstance.get('/pinpoint');
    console.log('Pinpoints fetched:', response.data); // Logging for vellykket pinpoint-henting
    return response.data.$values || response.data; // Tilpass etter JSON-struktur
  } catch (error) {
    console.error('Error fetching pinpoints:', error.response?.data || error.message); // Logging for feil ved pinpoint-henting
    throw error;
  }
};

export const createPinpoint = async (pinpoint) => {
  try {
    const response = await axiosInstance.post('/pinpoint', pinpoint);
    console.log('Pinpoint created:', response.data); // Logging for vellykket pinpoint-opprettelse
    return response.data;
  } catch (error) {
    console.error('Error creating pinpoint:', error.response?.data || error.message); // Logging for feil ved oppretting
    throw error;
  }
};

export const updatePinpoint = async (id, pinpoint) => {
  try {
    const response = await axiosInstance.put(`/pinpoint/${id}`, pinpoint);
    console.log('Pinpoint updated:', response.data); // Logging for vellykket oppdatering
    return response.data;
  } catch (error) {
    console.error('Error updating pinpoint:', error.response?.data || error.message); // Logging for feil ved oppdatering
    throw error;
  }
};

export const deletePinpoint = async (id) => {
  try {
    await axiosInstance.delete(`/pinpoint/${id}`);
    console.log('Pinpoint deleted with ID:', id); // Logging for vellykket sletting
  } catch (error) {
    console.error('Error deleting pinpoint:', error.response?.data || error.message); // Logging for feil ved sletting
    throw error;
  }
};

export const fetchComments = async (pinpointId) => {
  try {
    const response = await axiosInstance.get(`/pinpoint/${pinpointId}/comments`);
    console.log('Comments fetched for pinpoint:', pinpointId, response.data); // Logging for vellykket kommentar-henting
    return response.data;
  } catch (error) {
    console.error('Error fetching comments:', error.response?.data || error.message); // Logging for feil ved kommentar-henting
    throw error;
  }
};

export const addComment = async (pinpointId, commentText) => {
  try {
    const response = await axiosInstance.post(`/pinpoint/${pinpointId}/comments`, { text: commentText });
    console.log('Comment added to pinpoint:', pinpointId, response.data); // Logging for vellykket kommentar-leggelse
    return response.data;
  } catch (error) {
    console.error('Error adding comment:', error.response?.data || error.message); // Logging for feil ved kommentar-leggelse
    throw error;
  }
};
