import api from './api'

/**
 * Authentication service for login, registration, and logout.
 */

const authService = {
    register: async (userData) => {
        try {
            const response = await api.post('/Auth/register', userData);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Registration failed';
        }
    },

    login: async (credentials) => {
        try {
            const response = await api.post('/Auth/login', credentials);
            const {token, ...user } = response.data;

            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));

            return response.data;       
        } catch (error) {
            throw error.response?.data?.message || 'Login failed';
        }
    } ,

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },

    getCurrentUser: () => {
        const userStr = localStorage.getItem('user');
        return userStr ? JSON.parse(userStr) : null;
    },

    isAuthenticated: () => {
        return !!localStorage.getItem('token');
    },

    checkEmail: async (email) => {
        try {
            const response = await api.get(`/Auth/check-email?email=${email}`);
            return response.data.exists;    
        } catch (error) {
            console.error('Error checking email:', error);
            return false;  
        }
    }
};

export default authService;