import { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import doctorService from '../../services/doctorService';
import Button from '../../components/common/Button';
import { Trash2, Plus } from 'lucide-react';

const ManageSchedulePage = () => {
    const [availabilities, setAvailabilities] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showAddForm, setShowAddForm] = useState(false);

    const [newAvailability, setNewAvailability] = useState({
        dayOfWeek: '',
        startTime: '',
        endTime: ''
    });

    const daysOfWeek = [
        { value: 0, label: 'Sunday' },
        { value: 1, label: 'Monday' },
        { value: 2, label: 'Tuesday' },
        { value: 3, label: 'Wednesday' },
        { value: 4, label: 'Thursday' },
        { value: 5, label: 'Friday' },
        { value: 6, label: 'Saturday' }
    ];

    useEffect(() => {
        fetchAvailability();
    }, []);

    const fetchAvailability = async () => {
        try {
            // Get current user's doctor profile first
            const user = JSON.parse(localStorage.getItem('user'));
            const availability = await doctorService.getDoctorAvailability(user.userId);
            setAvailabilities(availability);
        } catch (error) {
            toast.error(error || 'Failed to load availability');
        } finally {
            setLoading(false);
        }
    };

    const handleAddAvailability = async (e) => {
        e.preventDefault();

        try {
            await doctorService.addAvailability({
                daysOfWeek: parseInt(newAvailability.dayOfWeek),
                startTime: newAvailability.startTime + ':00', // Convert to HH:MM:SS
                endTime: newAvailability.endTime + ':00'
            });

            toast.success('Availability added successfully!');
            setShowAddForm(false);
            setNewAvailability({ dayOfWeek: '', startTime: '', endTime: '' });
            fetchAvailability();
        } catch (error) {
            toast.error(error || 'Failed to add availability');
        }
    };

    const handleDeleteAvailability = async (availabilityId) => {
        if (!window.confirm('Are you sure you want to delete this availability?')) return;

        try {
            await doctorService.deleteAvailability(availabilityId);
            toast.success('Availability deleted');
            fetchAvailability();
        } catch (error) {
            toast.error(error || 'Failed to delete availability');
        }
    };

    if (loading) return <div className="page-container">Loading...</div>;

  return (
    <div className="page-container">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-900">Manage Schedule</h1>
        <Button onClick={() => setShowAddForm(!showAddForm)} variant="primary">
          <Plus size={18} />
          Add Availability
        </Button>
      </div>

      {/* Add Availability Form */}
      {showAddForm && (
        <div className="card mb-6">
          <h2 className="text-xl font-semibold mb-4">Add New Availability</h2>
          <form onSubmit={handleAddAvailability} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Day of Week
              </label>
              <select
                value={newAvailability.dayOfWeek}
                onChange={(e) => setNewAvailability({ ...newAvailability, dayOfWeek: e.target.value })}
                className="input-field"
                required
              >
                <option value="">Select day</option>
                {daysOfWeek.map(day => (
                  <option key={day.value} value={day.value}>{day.label}</option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Start Time
                </label>
                <input
                  type="time"
                  value={newAvailability.startTime}
                  onChange={(e) => setNewAvailability({ ...newAvailability, startTime: e.target.value })}
                  className="input-field"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  End Time
                </label>
                <input
                  type="time"
                  value={newAvailability.endTime}
                  onChange={(e) => setNewAvailability({ ...newAvailability, endTime: e.target.value })}
                  className="input-field"
                  required
                />
              </div>
            </div>

            <div className="flex gap-2">
              <Button type="submit" variant="primary">Add Availability</Button>
              <Button type="button" variant="outline" onClick={() => setShowAddForm(false)}>
                Cancel
              </Button>
            </div>
          </form>
        </div>
      )}

      {/* Current Availability */}
      <div className="card">
        <h2 className="text-xl font-semibold mb-4">Current Availability</h2>
        
        {availabilities.length === 0 ? (
          <div className="text-center py-8 text-gray-500">
            <p>No availability set. Add your working hours above.</p>
          </div>
        ) : (
          <div className="space-y-2">
            {availabilities.map((availability) => (
              <div key={availability.availabilityId} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                <div>
                  <span className="font-medium">{availability.dayName}</span>
                  <span className="text-gray-600 ml-4">
                    {availability.startTime.substring(0, 5)} - {availability.endTime.substring(0, 5)}
                  </span>
                </div>
                <button
                  onClick={() => handleDeleteAvailability(availability.availabilityId)}
                  className="text-red-600 hover:text-red-800"
                >
                  <Trash2 size={18} />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default ManageSchedulePage;