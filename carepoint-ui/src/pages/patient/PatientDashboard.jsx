import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import appointmentService from '../../services/appointmentService';
import { Calandar, Clock, FileText } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { format } from 'date-fns';
import { toast } from "react-toastify";

const PatientDashboard = () => {
    const { user } = useAuth();
    const [loading, setLoading] = useState(true);
    const [appointments, setAppointments] = useState([]);
    const [stats, setStats] = useState({
        total: 0,
        upcoming: 0,
        completed: 0
    });

    useEffect(() => {
        fetchAppointments();
    }, []);

    const fetchAppointments = async () => {
        try {
            const data = await appointmentService.getPatientAppointments();
            setAppointments(data);

            // Calculate stats
            const now = new Date();
            setStats({
                total: data.length,
                upcoming: data.filter(a => {
                    const apptDate = new Date(a.appointmentDate);
                    return apptDate >= now && a.status === 'Scheduled';
                }).length,
                completed: data.filter(a => a.status === 'Completed').length
            });
        } catch (error) {
            toast.error(error || 'Failed to load appointments');
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="page-container flex justify-center items-center min-h-screen">
                <LoadingSpinner size="large" />
            </div>
        );
    }

    const upcomingAppointments = appointments
        .filter(a => {
            const apptDate = new Date(a.appointmentDate);
            return apptDate >= new Date() && a.status === 'Scheduled';
        })
        .slice(0, 5);

  return (
    <div className="page-container">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, {user.firstName}!
        </h1>
        <p className="text-gray-600 mt-2">Here's an overview of your healthcare activities</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{stats.total}</p>
            </div>
            <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
              <Calendar className="w-6 h-6 text-primary-600" />
            </div>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Upcoming</p>
              <p className="text-3xl font-bold text-gray-900">{stats.upcoming}</p>
            </div>
            <div className="w-12 h-12 bg-secondary-100 rounded-full flex items-center justify-center">
              <Clock className="w-6 h-6 text-secondary-600" />
            </div>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Completed</p>
              <p className="text-3xl font-bold text-gray-900">{stats.completed}</p>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
              <FileText className="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <Link to="/patient/book-appointment" className="card hover:shadow-lg transition-shadow">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-primary-100 rounded-lg flex items-center justify-center">
              <Calendar className="w-8 h-8 text-primary-600" />
            </div>
            <div>
              <h3 className="text-xl font-semibold text-gray-900">Book Appointment</h3>
              <p className="text-gray-600">Schedule a visit with a doctor</p>
            </div>
          </div>
        </Link>

        <Link to="/patient/prescriptions" className="card hover:shadow-lg transition-shadow">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-secondary-100 rounded-lg flex items-center justify-center">
              <FileText className="w-8 h-8 text-secondary-600" />
            </div>
            <div>
              <h3 className="text-xl font-semibold text-gray-900">Prescriptions</h3>
              <p className="text-gray-600">View your prescriptions</p>
            </div>
          </div>
        </Link>
      </div>

      {/* Upcoming Appointments */}
      <div className="card">
        <h2 className="text-2xl font-bold text-gray-900 mb-4">Upcoming Appointments</h2>
        {upcomingAppointments.length === 0 ? (
          <div className="text-center py-12">
            <Calendar className="w-16 h-16 text-gray-300 mx-auto mb-4" />
            <p className="text-gray-600 mb-4">No upcoming appointments</p>
            <Link to="/patient/book-appointment" className="btn-primary">
              Book Your First Appointment
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {upcomingAppointments.map((appointment) => (
              <div key={appointment.appointmentId} className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-lg text-gray-900">
                      Dr. {appointment.doctorName}
                    </h3>
                    <p className="text-gray-600 text-sm">{appointment.specialtyName}</p>
                    <div className="flex items-center gap-4 mt-2 text-sm text-gray-500">
                      <span className="flex items-center gap-1">
                        <Calendar size={16} />
                        {format(new Date(appointment.appointmentDate), 'MMM dd, yyyy')}
                      </span>
                      <span className="flex items-center gap-1">
                        <Clock size={16} />
                        {appointment.appointmentTime}
                      </span>
                    </div>
                  </div>
                  <span className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm">
                    {appointment.status}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>    
  );    
};

export default PatientDashboard;