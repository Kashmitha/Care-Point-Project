import { useEffect, useState } from 'react';
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
            const user = JSON.parse(localStorage.getItem('user'));
            const doctorId = user?.userId;

            if (!doctorId) throw new Error();

            const data = await doctorService.getDoctorAvailability(doctorId);
            setAvailabilities(Array.isArray(data) ? data : []);
        } catch {
            toast.error('Failed to fetch availabilities');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAvailabilities();
    }, []);

    const deleteAvailability = async (id) => {
        if (!window.confirm('Are you sure you want to delete this availability?')) return;

        try {
            await doctorService.deleteAvailability(id);
            toast.success('Availability deleted successfully');
            fetchAvailabilities();
        } catch {
            toast.error('Failed to delete availability');
        }
    };

    if (loading) {
        return <LoadingSpinner size="large" />;
    }

  return (
    <div className="page-container">
      <h1 className="text-3xl font-bold mb-6">My Schedule</h1>

      <div className="grid lg:grid-cols-3 gap-6">
        <AddAvailability onSuccess={fetchAvailabilities} />

        <div className="lg:col-span-2 card">
          {availabilities.length === 0 ? (
            <p className="text-center text-gray-500 py-6">No availability set</p>
          ) : (
            availabilities.map(a => (
              <div key={a.availabilityId} className="flex justify-between p-4 border rounded mb-2">
                <div className="flex gap-3 items-center">
                  <Clock />
                  <div>
                    <p className="font-medium">{a.dayName}</p>
                    <p className="text-sm">
                      {a.startTime?.slice(0, 5)} - {a.endTime?.slice(0, 5)}
                    </p>
                  </div>
                </div>
                <button onClick={() => deleteAvailability(a.availabilityId)}>
                  <Trash2 className="text-red-600" />
                </button>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );    
};

export default DoctorSchedule;