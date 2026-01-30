import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import doctorService from "../../services/doctorService";
import Button from "../common/Button";

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
            const availabilityData = {
                dayOfWeek: Number(data.dayOfWeek),
                startTime: `${data.startTime}:00`,
                endTime: `${data.endTime}:00`
            };

            await doctorService.addAvailability(availabilityData);
            toast.success('Availability added successfully');
            reset();
            onSuccess?.();
        } catch (error) {
            toast.error(error.response?.data?.message || 'Failed to add availability');
        } finally {
            setLoading(false);
        }
    };

  return (
    <div className="card">
      <h3 className="text-xl font-bold mb-4">Add Availability</h3>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block mb-1 font-medium">Day</label>
          <select
            {...register('dayOfWeek', { required: 'Day is required' })}
            className="input-field"
          >
            <option value="">Select Day</option>
            {daysOfWeek.map(d => (
              <option key={d.value} value={d.value}>{d.label}</option>
            ))}
          </select>
          {errors.dayOfWeek && <p className="text-red-600 text-sm">{errors.dayOfWeek.message}</p>}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block mb-1 font-medium">Start Time</label>
            <input
              type="time"
              {...register('startTime', { required: 'Start time required' })}
              className="input-field"
            />
          </div>

          <div>
            <label className="block mb-1 font-medium">End Time</label>
            <input
              type="time"
              {...register('endTime', { required: 'End time required' })}
              className="input-field"
            />
          </div>
        </div>

        <Button type="submit" loading={loading} className="w-full">
          Add Availability
        </Button>
      </form>
    </div>
  );    
}
export default AddAvailability;
