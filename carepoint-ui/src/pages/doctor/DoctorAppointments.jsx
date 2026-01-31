import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import appointmentService from '../../services/appointmentService';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import Button from '../../components/common/Button';
import { Calendar, Clock, User, CheckCircle, FileText } from 'lucide-react';
import { format } from 'date-fns';

const DoctorAppointments = () => {
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState('all'); // all, upcoming, completed

    const fetchAppointments = async () => {
        try {
            const data = await appointmentService.getDoctorAppointments();
            setAppointments(data);
        } catch (error) {
            toast.error(error || 'Failed to load appointments');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAppointments();
    }, []);

    const handleComplete = async (appointmentId) => {
        if (!window.confirm('Mark this appointment as completed?')) return;

        try {
            await appointmentService.completeAppointment(appointmentId, 'Appointment completed');
            toast.success('Appointment marked as completed');
            fetchAppointments();
        } catch (error) {
            toast.error(error || 'Failed to complete appointment');
        }
    };

    if (loading) {
        return (
            <div className="page-container flex justify-center items-center min-h-screen">
                <LoadingSpinner size="large" />
            </div>
        );
    }

    const filteredAppointments = appointments.filter(apt => {
        if (filter === 'upcoming') return apt.status === 'Scheduled' && new Date(apt.appointmentDate) >= new Date();
        if (filter === 'completed') return apt.status === 'Completed';
        return true;
    });

  return (
    <div className="page-container">
      <div className="mb-8">
        <h1 className="text-3xl font-bold">My Appointments</h1>
        <p className="text-gray-600 mt-2">Manage your patient appointments</p>
      </div>

      <div className="mb-6 flex gap-2">
        <button
          onClick={() => setFilter('all')}
          className={`px-4 py-2 rounded-lg font-medium ${
            filter === 'all'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          All ({appointments.length})
        </button>
        <button
          onClick={() => setFilter('upcoming')}
          className={`px-4 py-2 rounded-lg font-medium ${
            filter === 'upcoming'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Upcoming ({appointments.filter(a => a.status === 'Scheduled').length})
        </button>
        <button
          onClick={() => setFilter('completed')}
          className={`px-4 py-2 rounded-lg font-medium ${
            filter === 'completed'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Completed ({appointments.filter(a => a.status === 'Completed').length})
        </button>
      </div>

      {filteredAppointments.length === 0 ? (
        <div className="card text-center py-12">
          <Calendar className="mx-auto text-gray-300 mb-4" size={64} />
          <p className="text-gray-600">No appointments found</p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredAppointments.map((appointment) => (
            <div key={appointment.appointmentId} className="card">
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <div className="flex items-center gap-3 mb-3">
                    <User className="text-primary-600" size={24} />
                    <div>
                      <h3 className="font-bold text-lg">{appointment.patientName}</h3>
                      <div className="flex items-center gap-4 text-sm text-gray-600 mt-1">
                        <span className="flex items-center gap-1">
                          <Calendar size={14} />
                          {format(new Date(appointment.appointmentDate), 'MMM dd, yyyy')}
                        </span>
                        <span className="flex items-center gap-1">
                          <Clock size={14} />
                          {appointment.appointmentTime.substring(0, 5)}
                        </span>
                      </div>
                    </div>
                  </div>

                  {appointment.reasonForVisit && (
                    <div className="p-3 bg-gray-50 rounded-lg mb-3">
                      <p className="text-sm text-gray-700">
                        <strong>Reason:</strong> {appointment.reasonForVisit}
                      </p>
                    </div>
                  )}

                  <div className="flex items-center gap-2">
                    <span
                      className={`px-3 py-1 rounded-full text-sm font-medium ${
                        appointment.status === 'Scheduled'
                          ? 'bg-blue-100 text-blue-800'
                          : appointment.status === 'Completed'
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                      }`}
                    >
                      {appointment.status}
                    </span>
                  </div>
                </div>

                <div className="flex flex-col gap-2">
                  {appointment.status === 'Scheduled' && (
                    <Button
                      variant="secondary"
                      onClick={() => handleComplete(appointment.appointmentId)}
                    >
                      <CheckCircle size={18} />
                      Complete
                    </Button>
                  )}
                  {appointment.status === 'Completed' && (
                    <Link to={`/doctor/prescription/${appointment.appointmentId}`}>
                      <Button variant="primary">
                        <FileText size={18} />
                        Write Prescription
                      </Button>
                    </Link>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default DoctorAppointments;