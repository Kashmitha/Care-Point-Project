import { useState, useEffect } from 'react';
import adminService from '../../services/adminService';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import Button from '../../components/common/Button';
import { toast } from 'react-toastify';
import { CheckCircle, XCircle } from 'lucide-react';

const ManageDoctors = () => {
    const [loading, setLoading] = useState(true);
    const [doctors, setDoctors] = useState([]);

    useEffect(() => {
        fetchPendingDoctors();
    }, []);

    const fetchPendingDoctors = async () => {
        try {
            const data = await adminService.getPendingDoctors();
            setDoctors(data);
        } catch (error) {
            toast.error(error || 'Failed to load doctors');
        } finally {
            setLoading(false);
        }
    };

    const handleApprove = async (doctorId) => {
        try {
            await adminService.approveDoctor(doctorId);
            toast.success('Doctor approved successfully');
            fetchPendingDoctors();
        } catch (error) {
            toast.error(error || 'Failed to approve doctor');
        }
    };

    const handleReject = async (doctorId) => {
        if (!window.confirm('Are you sure you want to reject this doctor?')) return;

        try {
            await adminService.rejectDoctor(doctorId);
            toast.success('Doctor rejected');
            fetchPendingDoctors();
        } catch (error) {
            toast.error(error || 'Failed to reject doctor');
        }
    };

    if (loading) {
        return (
        <div className="page-container flex justify-center items-center min-h-screen">
            <LoadingSpinner size="large" />
        </div>
        );
    }
    return (
        <div className="page-container">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">Pending Doctor Approvals</h1>

        {doctors.length === 0 ? (
            <div className="card text-center py-12">
            <p className="text-gray-600">No pending doctor approvals</p>
            </div>
        ) : (
            <div className="grid grid-cols-1 gap-6">
            {doctors.map((doctor) => (
                <div key={doctor.doctorId} className="card">
                <div className="flex justify-between items-start">
                    <div className="flex-1">
                    <h3 className="text-xl font-semibold">
                        Dr. {doctor.firstName} {doctor.lastName}
                    </h3>
                    <p className="text-gray-600">{doctor.specialtyName}</p>
                    
                    <div className="grid grid-cols-2 gap-4 mt-4 text-sm">
                        <div>
                        <p className="text-gray-500">Email</p>
                        <p className="font-medium">{doctor.email}</p>
                        </div>
                        <div>
                        <p className="text-gray-500">Phone</p>
                        <p className="font-medium">{doctor.phoneNumber || 'N/A'}</p>
                        </div>
                        <div>
                        <p className="text-gray-500">License</p>
                        <p className="font-medium">{doctor.licenseNumber}</p>
                        </div>
                        <div>
                        <p className="text-gray-500">Experience</p>
                        <p className="font-medium">{doctor.yearsOfExperience} years</p>
                        </div>
                        <div>
                        <p className="text-gray-500">Qualification</p>
                        <p className="font-medium">{doctor.qualification}</p>
                        </div>
                    </div>
                    </div>

                    <div className="flex gap-2">
                    <Button
                        variant="secondary"
                        onClick={() => handleApprove(doctor.doctorId)}
                    >
                        <CheckCircle size={18} />
                        Approve
                    </Button>
                    <Button
                        variant="danger"
                        onClick={() => handleReject(doctor.doctorId)}
                    >
                        <XCircle size={18} />
                        Reject
                    </Button>
                    </div>
                </div>
                </div>
            ))}
            </div>
        )}
        </div>
    );
};

export default ManageDoctors;