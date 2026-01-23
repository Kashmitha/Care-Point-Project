import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import doctorService from '../../services/doctorService';
import appointmentService from '../../services/appointmentService';
import { Search, Calendar } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import Button from '../../components/common/Button';
import Input from '../../components/common/Input';
import { toast } from 'react-toastify';

const BookAppointmentPage = () => {
    const navigate = useNavigate();
    const [doctors, setDoctors] = useState([]);
    const [loading, setLoading] = useState(false);
    const [selectedDoctor, setSelectedDoctor] = useState(null);
    const [bookingStep, setBookingStep] = useState(1); // 1: Search, 2: Search Time

    const { register, handleSubmit } = useForm();

    const searchDoctors = async (data) => {
        setLoading(true);
        try {
            const results = await doctorService.searchDoctors({
                searchTerm: data.searchTerm || '',
                specialtyId: data.specialtyId || null,
                pageNumber: 1,
                pageSize: 10
            });
            setDoctors(results);
        } catch (error) {
            toast.error(error || 'Failed to search doctors');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        searchDoctors({});
    }, []);

    const handleSelectDoctor = (doctor) => {
        setSelectedDoctor(doctor);
        setBookingStep(2);
    };

    const handleBookAppointment = async (appointmentData) => {
        try {
            await appointmentService.createAppointment({
                doctorId: selectedDoctor.doctorId,
                ...appointmentData
            });
            toast.success('Appointment booked successfully!');
            navigate('/patient/dashboard');
        } catch (error) {
            toast.error(error || 'Failed to book appointment');
        }
    };

    if (bookingStep === 2 && selectedDoctor) {
        return <AppointmentBookingForm doctor={selectedDoctor} onSubmit={handleBookAppointment} onBack={() => setBookingStep(1)} />;
    }

  return (
    <div className="page-container">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Book Appointment</h1>

      {/* Search Form */}
      <div className="card mb-8">
        <form onSubmit={handleSubmit(searchDoctors)} className="flex gap-4">
          <div className="flex-1">
            <Input
              {...register('searchTerm')}
              placeholder="Search by doctor name or specialty"
            />
          </div>
          <Button type="submit" variant="primary">
            <Search size={20} />
            Search
          </Button>
        </form>
      </div>

      {/* Doctor Results */}
      {loading ? (
        <div className="flex justify-center py-12">
          <LoadingSpinner size="large" />
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {doctors.map((doctor) => (
            <div key={doctor.doctorId} className="card hover:shadow-lg transition-shadow">
              <h3 className="text-xl font-semibold text-gray-900">
                Dr. {doctor.firstName} {doctor.lastName}
              </h3>
              <p className="text-gray-600 text-sm mb-2">{doctor.specialtyName}</p>
              <p className="text-gray-500 text-sm mb-4">{doctor.qualification}</p>
              <div className="flex justify-between items-center mb-4">
                <span className="text-gray-600">Experience:</span>
                <span className="font-semibold">{doctor.yearsOfExperience} years</span>
              </div>
              <div className="flex justify-between items-center mb-4">
                <span className="text-gray-600">Fee:</span>
                <span className="font-semibold text-primary-600">${doctor.consultationFee}</span>
              </div>
              <Button variant="primary" className="w-full" onClick={() => handleSelectDoctor(doctor)}>
                Book Appointment
              </Button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

// Appointment Booking Form Component
const AppointmentBookingForm = ({ doctor, onSubmit, onBack }) => {
    const [loading, setLoading] = useState(false);
    const { register, handleSubmit, formState: { errors } } = useForm();

    const handleFormSubmit = async (data) => {
        setLoading(true);
        try {
            // Format time as TimeSpan (HH:mm:ss)
            const timespan = data.appointmentTime + ':00';
            await onSubmit ({
                appointmentDate: data.appointmentDate,
                appointmentTime: timespan,
                reasonForVisit: data.reasonForVisit
            });
        } finally {
            setLoading(false);
        }
    };

  return (
    <div className="page-container">
      <button onClick={onBack} className="text-primary-600 mb-4 hover:underline">
        ← Back to doctors
      </button>

      <div className="card max-w-2xl mx-auto">
        <h2 className="text-2xl font-bold mb-6">Book Appointment with Dr. {doctor.firstName} {doctor.lastName}</h2>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
          <Input
            {...register('appointmentDate', { required: 'Date is required' })}
            label="Appointment Date"
            type="date"
            min={new Date().toISOString().split('T')[0]}
            error={errors.appointmentDate?.message}
          />

          <Input
            {...register('appointmentTime', { required: 'Time is required' })}
            label="Appointment Time"
            type="time"
            error={errors.appointmentTime?.message}
          />

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Reason for Visit
            </label>
            <textarea
              {...register('reasonForVisit')}
              rows="4"
              className="input-field"
              placeholder="Describe your symptoms or reason for visit..."
            />
          </div>

          <Button type="submit" variant="primary" loading={loading} className="w-full">
            Confirm Booking
          </Button>
        </form>
      </div>
    </div>
  );
};

export default BookAppointmentPage;