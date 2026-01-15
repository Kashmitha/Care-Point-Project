import api from './api';

const doctorService = {
    searchDoctors: async (searchParams) => {
        try {
            const response = await api.get('/Doctors/search', { params: searchParams });
            return response.data;   
        } catch (error) {
            throw error.response?.data?.message || 'Failed to search doctors';
        }
    },

    getDoctorProfile: async (doctorId) => {
        try {
            const response = await api.get(`/Doctors/${doctorId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get doctor profile';
        }
    },

    getDoctorAvailability: async (doctorId) => {
        try {
            const response = await api.get(`/Doctors/${doctorId}/availability`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get availability';
        }
    },

    addAvailability: async (availabilityData) => {
        try {
            const response = await api.post('/Doctors/availability', availabilityData);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to add availability';
        }
    },

    deleteAvailability: async (availabilityId) => {
        try {
            await api.delete(`/Doctors/availability/${availabilityId}`);
        } catch (error) {
            throw error.response?.data?.message || 'Failed to delete availability';
        }
    },

    updateProfile: async (profileData) => {
        try {
            const response = await api.put('/Doctors/profile', profileData);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update profile';
        }
    }
};

export default doctorService;
