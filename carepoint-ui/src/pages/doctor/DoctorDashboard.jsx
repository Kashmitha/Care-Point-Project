import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { Calendar, Clock, Users, CheckCircle } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import appointmentService from '../../services/appointmentService';
import { toast } from 'react-toastify';
import { format, isToday } from 'date-fns';

const DoctorDashboard = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [appointments, setAppointments] = useState([]);

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

  if (loading) {
    return (
      <div className="page-container flex justify-center items-center min-h-screen">
        <LoadingSpinner size="large" />
      </div>
    );
  }

  // Calculate stats
  const todayAppointments = appointments.filter(apt => 
    isToday(new Date(apt.appointmentDate)) && apt.status === 'Scheduled'
  );

  const upcomingAppointments = appointments.filter(apt => 
    apt.status === 'Scheduled' && new Date(apt.appointmentDate) >= new Date()
  ).sort((a, b) => new Date(a.appointmentDate) - new Date(b.appointmentDate));

  const stats = {
    totalAppointments: appointments.length,
    todayAppointments: todayAppointments.length,
    upcomingAppointments: upcomingAppointments.length,
    completedAppointments: appointments.filter(a => a.status === 'Completed').length
  };

  return (
    <div className="page-container">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome, Dr. {user.lastName}!
        </h1>
        <p className="text-gray-600 mt-2">Here's your practice overview</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{stats.totalAppointments}</p>
            </div>
            <Calendar className="text-primary-600" size={32} />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Today's Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{stats.todayAppointments}</p>
            </div>
            <Clock className="text-orange-600" size={32} />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Upcoming</p>
              <p className="text-3xl font-bold text-gray-900">{stats.upcomingAppointments}</p>
            </div>
            <Users className="text-secondary-600" size={32} />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Completed</p>
              <p className="text-3xl font-bold text-gray-900">{stats.completedAppointments}</p>
            </div>
            <CheckCircle className="text-green-600" size={32} />
          </div>
        </div>
      </div>

      {/* Today's Appointments */}
      <div className="mb-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-4">Today's Schedule</h2>
        
        {todayAppointments.length === 0 ? (
          <div className="card text-center py-12">
            <Clock className="mx-auto text-gray-300 mb-4" size={48} />
            <p className="text-gray-600">No appointments scheduled for today</p>
          </div>
        ) : (
          <div className="space-y-4">
            {todayAppointments.map(appointment => (
              <div key={appointment.appointmentId} className="card">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <h3 className="font-semibold text-lg text-gray-900">
                      {appointment.patientName}
                    </h3>
                    <p className="text-gray-600 mt-1">
                      <Clock size={16} className="inline mr-1" />
                      {appointment.appointmentTime.substring(0, 5)}
                    </p>
                    {appointment.reasonForVisit && (
                      <p className="text-sm text-gray-600 mt-2">
                        <strong>Reason:</strong> {appointment.reasonForVisit}
                      </p>
                    )}
                  </div>
                  <Link
                    to={`/doctor/appointments/${appointment.appointmentId}`}
                    className="btn-primary"
                  >
                    View Details
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Upcoming Appointments */}
      <div>
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-bold text-gray-900">Upcoming Appointments</h2>
          <Link to="/doctor/appointments" className="text-primary-600 hover:text-primary-700">
            View All
          </Link>
        </div>

        {upcomingAppointments.length === 0 ? (
          <div className="card text-center py-12">
            <Calendar className="mx-auto text-gray-300 mb-4" size={48} />
            <p className="text-gray-600">No upcoming appointments</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {upcomingAppointments.slice(0, 6).map(appointment => (
              <div key={appointment.appointmentId} className="card">
                <div className="mb-3">
                  <h3 className="font-semibold text-gray-900">
                    {appointment.patientName}
                  </h3>
                  <p className="text-sm text-gray-600">
                    {format(new Date(appointment.appointmentDate), 'MMM dd, yyyy')} at{' '}
                    {appointment.appointmentTime.substring(0, 5)}
                  </p>
                </div>
                {appointment.reasonForVisit && (
                  <p className="text-sm text-gray-600 mb-3">
                    {appointment.reasonForVisit}
                  </p>
                )}
                <Link
                  to={`/doctor/appointments/${appointment.appointmentId}`}
                  className="text-primary-600 hover:text-primary-700 text-sm font-medium"
                >
                  View Details →
                </Link>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default DoctorDashboard;
