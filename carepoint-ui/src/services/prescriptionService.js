import api from './api';

const prescriptionService = {
    createPrescription: async (prescriptionData) => {
        try {
            const responses = await api.post('/Prescriptions', prescriptionData);
            return responses.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to create prescription';
        }
    },

    getPatientPrescriptions: async () => {
        try {
            const response = await api.get('/Prescriptions/patient');
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get prescriptions';
        }
    },

    getPrescription: async (prescriptionId) => {
        try {
            const response = await api.get(`/Prescriptions/${prescriptionId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get prescription';
        }
    },

    getPrescriptionByAppointment: async (appointmentId) => {
        try {
            const response = await api.get(`/Prescriptions/appointment/${appointmentId}`);
            return response.data;
        } catch (error) {
            throw error.response?.data?.message || 'Failed to get prescription';
        }
    }
};

export default prescriptionService;