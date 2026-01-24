import { useState, useEffect } from 'react';
import { useAuth } from '../../hooks/useAuth';
import appointmentService from '../../services/appointmentService';
import { Calendar, Clock, Users } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { toast } from 'react-toastify';

const DoctorDashboard = () => {
    const { user } = useAuth();
    const [ loading, setLoading ] = useState(true);
    const [ appointments, setAppointments ] = useState([]);

    useEffect(() => {
        fetchAppointments();
    }, []);

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

    if(loading)
    {
        return (
            <div className="page-container flex justify-center items-center min-h-screen">
                <LoadingSpinner size="large" />
            </div>
        );
    }

    const todayAppointments = appointments.filter(a => {
        const apptDate = new Date(a.appointmentDate);
        const today = new Date();
        return apptDate.toDateString() === today.toDateString() && a.status === 'Scheduled';
    });

  return (
    <div className="page-container">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome, Dr. {user.firstName}!
        </h1>
        <p className="text-gray-600 mt-2">Here's your schedule for today</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{appointments.length}</p>
            </div>
            <Calendar className="w-12 h-12 text-primary-600" />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Today's Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{todayAppointments.length}</p>
            </div>
            <Clock className="w-12 h-12 text-secondary-600" />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Patients</p>
              <p className="text-3xl font-bold text-gray-900">
                {new Set(appointments.map(a => a.patientId)).size}
              </p>
            </div>
            <Users className="w-12 h-12 text-yellow-600" />
          </div>
        </div>
      </div>

      {/* Today's Appointments */}
      <div className="card">
        <h2 className="text-2xl font-bold text-gray-900 mb-4">Today's Appointments</h2>
        {todayAppointments.length === 0 ? (
          <p className="text-center text-gray-600 py-8">No appointments for today</p>
        ) : (
          <div className="space-y-4">
            {todayAppointments.map((appointment) => (
              <div key={appointment.appointmentId} className="border border-gray-200 rounded-lg p-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-lg">{appointment.patientName}</h3>
                    <p className="text-gray-600 text-sm">{appointment.reasonForVisit}</p>
                    <div className="flex items-center gap-2 mt-2 text-sm text-gray-500">
                      <Clock size={16} />
                      {appointment.appointmentTime}
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

export default DoctorDashboard;