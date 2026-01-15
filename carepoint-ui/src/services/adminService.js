import api from './api';

const adminService = {
    getDashboardStats: async () => {
        try {
            const response = await api.get('/Admin/stats');
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get statistics';
        }
    },

    getPendingDoctors: async () => {
        try {
            const response = await api.get('/Admin/pending-doctors');
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get pending doctors';
        }
    },

    approveDoctor: async (doctorId) => {
        try {
            const response = await api.post(`/Admin/approve-doctor/${doctorId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to approve doctor';
        }
    },

    rejectDoctor: async (doctorId) => {
        try {
            const response = await api.post(`/Admin/reject-doctor/${doctorId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to reject doctor';
        }
    }
};

export default adminService;