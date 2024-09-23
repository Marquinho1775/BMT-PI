// services/authService.js
import axios from 'axios';

const API_URL = 'https://localhost:7189/api/Account/';

export const login = async (username, password) => {
    const response = await axios.post(API_URL + 'login', { username, password });
    if (response.data.token) {
        localStorage.setItem('token', response.data.token);
    }
    return response.data;
};

export const logout = () => {
    localStorage.removeItem('token');
};

export const getCurrentUser = () => {
    return localStorage.getItem('token');
};
