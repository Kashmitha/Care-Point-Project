import { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import appointmentService from '../../services/appointmentService';
import { Calendar, Clock, X } from 'lucide-react';
import { format } from 'date-fns';

const AppointmentsPage = () => {
    const [appointments, setAppointments] = useState([]);
    const [activeTab, setActiveTab] = useState('Upcoming'); // 'Upcoming', 'Past', 'Cancelled'
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchAppointments();
    }, []);

    const fetchAppointments = async () => {
        try {
            const data = await appointmentService.getPatientAppointments();
            setAppointments(data);
        } catch (error) {
            toast.error(error || 'Failed to load appointments');
        } finally {
            setLoading(false);
        }
    };

    const handleCancelAppointment = async (appointmentId) => {
        if (!window.confirm('Are you sure you want to cancel this appointment?')) return;

        try {
            await appointmentService.cancelAppointment(appointmentId, 'Cancelled by patient');
            toast.success('Appointment cancelled');
            fetchAppointments();
        } catch (error) {
            toast.error(error || 'Failed to cancel appointment');
        }
    };

    const filterAppointments = () => {
        const now = new Date();

        switch (activeTab) {
        case 'upcoming':
            return appointments.filter(apt => {
            const aptDate = new Date(apt.appointmentDate);
            return apt.status === 'Scheduled' && aptDate >= now;
            });
        case 'past':
            return appointments.filter(apt => {
            const aptDate = new Date(apt.appointmentDate);
            return apt.status === 'Completed' || (apt.status === 'Scheduled' && aptDate < now);
            });
        case 'cancelled':
            return appointments.filter(apt => apt.status === 'Cancelled');
        default:
            return appointments;
        }
    };

    const filteredAppointments = filterAppointments();

  return (
    <div className="page-container">
      <h1 className="text-3xl font-bold text-gray-900 mb-6">My Appointments</h1>

      {/* Tabs */}
      <div className="flex space-x-4 mb-6 border-b">
        <button
          onClick={() => setActiveTab('upcoming')}
          className={`pb-2 px-4 font-medium ${
            activeTab === 'upcoming'
              ? 'border-b-2 border-primary-600 text-primary-600'
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          Upcoming ({appointments.filter(a => a.status === 'Scheduled').length})
        </button>
        <button
          onClick={() => setActiveTab('past')}
          className={`pb-2 px-4 font-medium ${
            activeTab === 'past'
              ? 'border-b-2 border-primary-600 text-primary-600'
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          Past ({appointments.filter(a => a.status === 'Completed').length})
        </button>
        <button
          onClick={() => setActiveTab('cancelled')}
          className={`pb-2 px-4 font-medium ${
            activeTab === 'cancelled'
              ? 'border-b-2 border-primary-600 text-primary-600'
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          Cancelled ({appointments.filter(a => a.status === 'Cancelled').length})
        </button>
      </div>

      {/* Appointments List */}
      {loading ? (
        <div>Loading...</div>
      ) : filteredAppointments.length === 0 ? (
        <div className="card text-center py-12">
          <Calendar className="w-16 h-16 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-600">No {activeTab} appointments</p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredAppointments.map((appointment) => (
            <div key={appointment.appointmentId} className="card">
              <div className="flex justify-between items-start">
                <div>
                  <h3 className="font-semibold text-lg">Dr. {appointment.doctorName}</h3>
                  <p className="text-gray-600">{appointment.specialtyName}</p>
                  
                  <div className="flex items-center gap-4 mt-2 text-sm text-gray-600">
                    <div className="flex items-center gap-1">
                      <Calendar size={16} />
                      {format(new Date(appointment.appointmentDate), 'MMM dd, yyyy')}
                    </div>
                    <div className="flex items-center gap-1">
                      <Clock size={16} />
                      {appointment.appointmentTime.substring(0, 5)}
                    </div>
                  </div>

                  {appointment.reasonForVisit && (
                    <p className="mt-2 text-sm text-gray-700">
                      <span className="font-medium">Reason:</span> {appointment.reasonForVisit}
                    </p>
                  )}
                </div>

                <div className="flex items-center gap-2">
                  <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                    appointment.status === 'Scheduled' ? 'bg-blue-100 text-blue-700' :
                    appointment.status === 'Completed' ? 'bg-green-100 text-green-700' :
                    'bg-red-100 text-red-700'
                  }`}>
                    {appointment.status}
                  </span>

                  {appointment.status === 'Scheduled' && (
                    <button
                      onClick={() => handleCancelAppointment(appointment.appointmentId)}
                      className="text-red-600 hover:text-red-800 p-2"
                      title="Cancel appointment"
                    >
                      <X size={20} />
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );    
}

export default AppointmentsPage;