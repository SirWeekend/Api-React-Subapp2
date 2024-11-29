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

export const fetchPinpoints = async () => {
  try {
    const response = await axiosInstance.get('/pinpoint');
    console.log('Pinpoints fetched:', response.data);
    return response.data.$values || response.data;
  } catch (error) {
    console.error('Error fetching pinpoints:', error.response?.data || error.message);
    throw error;
  }
};

export const createPinpoint = async (pinpoint) => {
  try {
    const response = await axiosInstance.post('/pinpoint', pinpoint);
    console.log('Pinpoint created:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error creating pinpoint:', error.response?.data || error.message);
    throw error;
  }
};

export const updatePinpoint = async (id, pinpoint) => {
  try {
    const response = await axiosInstance.put(`/pinpoint/${id}`, pinpoint);
    console.log('Pinpoint updated:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error updating pinpoint:', error.response?.data || error.message);
    throw error;
  }
};

export const deletePinpoint = async (id) => {
  try {
    await axiosInstance.delete(`/pinpoint/${id}`);
    console.log('Pinpoint deleted with ID:', id);
  } catch (error) {
    console.error('Error deleting pinpoint:', error.response?.data || error.message);
    throw error;
  }
};

export const loginUser = async (username, password) => {
  try {
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

export const registerUser = async (username, email, password) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/Bruker/Registrer`, { username, email, password });
    console.log('User registered successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error during registration:', error.response?.data || error.message);
    throw error;
  }
};

export const logoutUser = async () => {
  try {
    await axios.post(`${API_BASE_URL}/Bruker/Logout`);
    localStorage.removeItem('token');
    console.log('User logged out successfully');
  } catch (error) {
    console.error('Error during logout:', error.response?.data || error.message);
    throw error;
  }
};
