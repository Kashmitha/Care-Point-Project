import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import doctorService from '../../services/doctorService';
import Button from '../common/Button';
import Select from '../common/Select';

const AddAvailability = ({ onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const { register, handleSubmit, formState: { errors }, reset } = useForm();

  const daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' }
  ];

  const onSubmit = async (data) => {
    setLoading(true);
    try {
      // FIX: Convert to proper format
      const availabilityData = {
        dayOfWeek: parseInt(data.dayOfWeek), // Convert string to int
        startTime: data.startTime + ':00', // HH:mm:ss format
        endTime: data.endTime + ':00'
      };

      await doctorService.addAvailability(availabilityData);
      toast.success('Availability added successfully!');
      reset();
      if (onSuccess) onSuccess();
    } catch (error) {
      toast.error(error || 'Failed to add availability');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <h3 className="text-xl font-bold mb-4">Add Availability</h3>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Day of Week
          </label>
          <select
            {...register('dayOfWeek', { required: 'Day is required' })}
            className="input-field"
          >
            <option value="">Select Day</option>
            {daysOfWeek.map(day => (
              <option key={day.value} value={day.value}>
                {day.label}
              </option>
            ))}
          </select>
          {errors.dayOfWeek && (
            <p className="text-red-600 text-sm mt-1">{errors.dayOfWeek.message}</p>
          )}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Start Time
            </label>
            <input
              type="time"
              {...register('startTime', { required: 'Start time is required' })}
              className="input-field"
            />
            {errors.startTime && (
              <p className="text-red-600 text-sm mt-1">{errors.startTime.message}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              End Time
            </label>
            <input
              type="time"
              {...register('endTime', { required: 'End time is required' })}
              className="input-field"
            />
            {errors.endTime && (
              <p className="text-red-600 text-sm mt-1">{errors.endTime.message}</p>
            )}
          </div>
        </div>

        <Button type="submit" variant="primary" loading={loading} className="w-full">
          Add Availability
        </Button>
      </form>
    </div>
  );
};

export default AddAvailability;
