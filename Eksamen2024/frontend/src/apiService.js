import axios from 'axios';

const API_BASE_URL = 'http://localhost:5091/api';

const getToken = () => localStorage.getItem('token');

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    console.error('Error with request interceptor:', error);
    return Promise.reject(error);
  }
);

// Hent alle pinpoints
export const fetchPinpoints = async () => {
  try {
    const response = await axiosInstance.get('/pinpoint');
    console.log('API response:', response.data);

    const pinpoints = response.data.$values || response.data;
    console.log('Fetched pinpoints before cleaning:', pinpoints);

    // Filtrer ut ugyldige pinpoints (de som mangler nødvendig data)
    const cleanedPinpoints = pinpoints
      .filter((pinpoint) => pinpoint && pinpoint.pinpointId && pinpoint.name)
      .map((pinpoint) => {
        const { $ref, ...cleanedPinpoint } = pinpoint; // Fjern $ref hvis det finnes
        return cleanedPinpoint;
      });

    console.log('Cleaned pinpoints:', cleanedPinpoints);
    return cleanedPinpoints;
  } catch (error) {
    console.error('Error fetching pinpoints:', error.response?.data || error.message);
    throw error;
  }
};


// Opprett nytt pinpoint
export const createPinpoint = async (pinpoint) => {
  try {
    console.log('Sending pinpoint to server:', pinpoint);
    const response = await axiosInstance.post('/pinpoint', pinpoint);
    console.log('Pinpoint created successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error creating pinpoint:', error.response?.data || error.message);
    throw error;
  }
};

// Oppdater eksisterende pinpoint
export const updatePinpoint = async (id, pinpoint) => {
  try {
    console.log('Updating pinpoint with data:', pinpoint);
    const response = await axiosInstance.put(`/pinpoint/${id}`, pinpoint);
    console.log('Pinpoint updated successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error updating pinpoint:', error.response?.data || error.message);
    throw error;
  }
};


// Slett pinpoint
export const deletePinpoint = async (id) => {
  try {
    console.log('Deleting pinpoint with ID:', id);
    await axiosInstance.delete(`/pinpoint/${id}`);
    console.log('Pinpoint deleted successfully');
  } catch (error) {
    console.error('Error deleting pinpoint:', error.response?.data || error.message);
    throw error;
  }
};

// Logg inn bruker
export const loginUser = async (username, password) => {
  try {
    console.log('Logging in user:', username);
    const response = await axios.post(`${API_BASE_URL}/Bruker/Login`, { username, password });
    const token = response.data.token;
    if (token) {
      localStorage.setItem('token', token);
    }
    console.log('Login successful:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error during login:', error.response?.data || error.message);
    throw error;
  }
};

// Registrer ny bruker
export const registerUser = async (username, email, password) => {
  try {
    console.log('Registering user:', username);
    const response = await axios.post(`${API_BASE_URL}/Bruker/Registrer`, { username, email, password });
    console.log('User registered successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error during registration:', error.response?.data || error.message);
    throw error;
  }
};

// Logg ut bruker
export const logoutUser = async () => {
  try {
    console.log('Logging out user');
    await axios.post(`${API_BASE_URL}/Bruker/Logout`);
    localStorage.removeItem('token');
    console.log('User logged out successfully');
  } catch (error) {
    console.error('Error during logout:', error.response?.data || error.message);
    throw error;
  }
};
