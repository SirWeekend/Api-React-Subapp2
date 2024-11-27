import axios from 'axios';

const API_BASE_URL = 'http://localhost:5091/api/pinpoint';

// Henter alle pinpoints
export const fetchPinpoints = async () => {
    const response = await axios.get(API_BASE_URL);
    return response.data.$values;
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
