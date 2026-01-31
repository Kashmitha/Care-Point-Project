import { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import doctorService from '../../services/doctorService';
import AddAvailability from '../../components/doctor/AddAvailability';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { Trash2, Clock } from 'lucide-react';

const DoctorSchedule = () => {
  const [availabilities, setAvailabilities] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchAvailabilities = async () => {
    try {
      // Get current user's doctor profile first
      const user = JSON.parse(localStorage.getItem('user'));
      const doctorId = user.userId; // Assuming doctorId equals userId
      
      const data = await doctorService.getDoctorAvailability(doctorId);
      setAvailabilities(data);
    } catch (error) {
      toast.error(error || 'Failed to load schedule');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAvailabilities();
  }, []);

  const handleDelete = async (availabilityId) => {
    if (!window.confirm('Are you sure you want to delete this availability?')) {
      return;
    }

    try {
      await doctorService.deleteAvailability(availabilityId);
      toast.success('Availability deleted successfully');
      fetchAvailabilities(); // Refresh list
    } catch (error) {
      toast.error(error || 'Failed to delete availability');
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
      <h1 className="text-3xl font-bold mb-8">My Schedule</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-1">
          <AddAvailability onSuccess={fetchAvailabilities} />
        </div>

        <div className="lg:col-span-2">
          <div className="card">
            <h3 className="text-xl font-bold mb-4">Current Availability</h3>
            
            {availabilities.length === 0 ? (
              <p className="text-gray-500 text-center py-8">
                No availability set. Add your schedule to start accepting appointments.
              </p>
            ) : (
              <div className="space-y-3">
                {availabilities.map(availability => (
                  <div
                    key={availability.availabilityId}
                    className="flex items-center justify-between p-4 border border-gray-200 rounded-lg hover:bg-gray-50"
                  >
                    <div className="flex items-center gap-4">
                      <Clock className="text-primary-600" size={20} />
                      <div>
                        <p className="font-medium text-gray-900">
                          {availability.dayName}
                        </p>
                        <p className="text-sm text-gray-600">
                          {availability.startTime.substring(0, 5)} - {availability.endTime.substring(0, 5)}
                        </p>
                      </div>
                    </div>
                    <button
                      onClick={() => handleDelete(availability.availabilityId)}
                      className="text-red-600 hover:text-red-700 p-2"
                    >
                      <Trash2 size={18} />
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default DoctorSchedule;
