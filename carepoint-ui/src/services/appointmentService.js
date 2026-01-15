import api from './api';

const appointmentService = {
    createAppointment: async (appointmentData) => {
        try {
            const response = await api.post('/Appointments', appointmentData);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to create appointment';
        }
    },

    getPatientAppointments: async () => {
        try {
            const response = await api.get('/Appointments/patient');
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get appointments';
        }
    },

    getDoctorAppointments: async () => {
        try {
            const response = await api.get('/Appointments/doctor');
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get appointments';
        }
    },

    getAppointment: async (appointmentId) => {
        try {
            const response = await api.get(`/Appointments/${appointmentId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get appointment';
        }
    },

    updateAppointment: async (appointmentId, appointmentData) => {
        try {
            const response = await api.put(`/Appointments/${appointmentId}`, appointmentData);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to update appointment';
        }
    },

    cancelAppointment: async (appointmentId, reason) => {
        try {
            await api.post(`/Appointments/${appointmentId}/cancel`, { cancellationReason: reason });
        } catch (error) {
            throw error.response?.data?.message || 'Failed to cancel appointment';
        }
    },

    completeAppointment: async (appointmentId, notes) => {
        try {
            await api.post(`/Appointments/${appointmentId}/complete`, { notes });
        } catch (error) {
            throw error.response?.data?.message || 'Failed to complete appointment';
        }
    }
};

export default appointmentService;