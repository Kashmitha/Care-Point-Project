import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { Calendar, Clock, FileText, Pill } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import AppointmentCard from '../../components/patient/AppointmentCard';
import appointmentService from '../../services/appointmentService';
import prescriptionService from '../../services/prescriptionService';
import { toast } from 'react-toastify';

const PatientDashboard = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [appointments, setAppointments] = useState([]);
  const [prescriptions, setPrescriptions] = useState([]);

  const fetchData = async () => {
    try {
      const [appointmentsData, prescriptionsData] = await Promise.all([
        appointmentService.getPatientAppointments(),
        prescriptionService.getPatientPrescriptions()
      ]);
      
      setAppointments(appointmentsData);
      setPrescriptions(prescriptionsData);
    } catch (error) {
      toast.error(error || 'Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="page-container flex justify-center items-center min-h-screen">
        <LoadingSpinner size="large" />
      </div>
    );
  }

  // Filter upcoming appointments
  const upcomingAppointments = appointments
    .filter(apt => apt.status === 'Scheduled' && new Date(apt.appointmentDate) >= new Date())
    .sort((a, b) => new Date(a.appointmentDate) - new Date(b.appointmentDate))
    .slice(0, 3);

  const stats = {
    totalAppointments: appointments.length,
    upcomingAppointments: upcomingAppointments.length,
    prescriptions: prescriptions.length,
    completedAppointments: appointments.filter(a => a.status === 'Completed').length
  };

  return (
    <div className="page-container">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, {user.firstName}!
        </h1>
        <p className="text-gray-600 mt-2">Here's your health overview</p>
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
              <p className="text-gray-600 text-sm">Upcoming</p>
              <p className="text-3xl font-bold text-gray-900">{stats.upcomingAppointments}</p>
            </div>
            <Clock className="text-secondary-600" size={32} />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Prescriptions</p>
              <p className="text-3xl font-bold text-gray-900">{stats.prescriptions}</p>
            </div>
            <Pill className="text-purple-600" size={32} />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Completed</p>
              <p className="text-3xl font-bold text-gray-900">{stats.completedAppointments}</p>
            </div>
            <FileText className="text-green-600" size={32} />
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <Link to="/patient/book-appointment" className="card hover:shadow-lg transition-shadow">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-primary-100 rounded-lg flex items-center justify-center">
              <Calendar className="text-primary-600" size={32} />
            </div>
            <div>
              <h3 className="text-xl font-semibold text-gray-900">Book Appointment</h3>
              <p className="text-gray-600">Schedule with a doctor</p>
            </div>
          </div>
        </Link>

        <Link to="/patient/prescriptions" className="card hover:shadow-lg transition-shadow">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-purple-100 rounded-lg flex items-center justify-center">
              <Pill className="text-purple-600" size={32} />
            </div>
            <div>
              <h3 className="text-xl font-semibold text-gray-900">My Prescriptions</h3>
              <p className="text-gray-600">View all prescriptions</p>
            </div>
          </div>
        </Link>
      </div>

      {/* Upcoming Appointments */}
      <div className="mb-8">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-bold text-gray-900">Upcoming Appointments</h2>
          <Link to="/patient/appointments" className="text-primary-600 hover:text-primary-700">
            View All
          </Link>
        </div>

        {upcomingAppointments.length === 0 ? (
          <div className="card text-center py-12">
            <Calendar className="mx-auto text-gray-300 mb-4" size={48} />
            <p className="text-gray-600 mb-4">No upcoming appointments</p>
            <Link to="/patient/book-appointment" className="btn-primary">
              Book Your First Appointment
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {upcomingAppointments.map(appointment => (
              <AppointmentCard
                key={appointment.appointmentId}
                appointment={appointment}
                onUpdate={fetchData}
              />
            ))}
          </div>
        )}
      </div>

      {/* Recent Prescriptions */}
      <div>
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-bold text-gray-900">Recent Prescriptions</h2>
          <Link to="/patient/prescriptions" className="text-primary-600 hover:text-primary-700">
            View All
          </Link>
        </div>

        {prescriptions.length === 0 ? (
          <div className="card text-center py-12">
            <Pill className="mx-auto text-gray-300 mb-4" size={48} />
            <p className="text-gray-600">No prescriptions yet</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {prescriptions.slice(0, 4).map(prescription => (
              <div key={prescription.prescriptionId} className="card">
                <div className="flex justify-between items-start mb-3">
                  <h3 className="font-semibold text-gray-900">
                    Dr. {prescription.doctorName}
                  </h3>
                  <span className="text-sm text-gray-500">
                    {new Date(prescription.createdAt).toLocaleDateString()}
                  </span>
                </div>
                {prescription.diagnosis && (
                  <p className="text-sm text-gray-600 mb-2">
                    <strong>Diagnosis:</strong> {prescription.diagnosis}
                  </p>
                )}
                <div className="mt-3">
                  <p className="text-sm font-medium text-gray-700 mb-2">Medications:</p>
                  <ul className="space-y-1">
                    {prescription.medications.slice(0, 2).map(med => (
                      <li key={med.medicationId} className="text-sm text-gray-600">
                        • {med.medicineName} - {med.dosage}
                      </li>
                    ))}
                    {prescription.medications.length > 2 && (
                      <li className="text-sm text-primary-600">
                        +{prescription.medications.length - 2} more
                      </li>
                    )}
                  </ul>
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
