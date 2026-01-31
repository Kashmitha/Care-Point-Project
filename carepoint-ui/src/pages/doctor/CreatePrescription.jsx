import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm, useFieldArray } from 'react-hook-form';
import { toast } from 'react-toastify';
import prescriptionService from '../../services/prescriptionService';
import appointmentService from '../../services/appointmentService';
import Button from '../../components/common/Button';
import { Plus, Trash2 } from 'lucide-react';

const CreatePrescription = () => {
    const { appointmentId } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [appointment, setAppointment] = useState(null);

    const { register, control, handleSubmit, formState: { errors } } = useForm({
        defaultValues: {
            diagnosis: '',
            instructions: '',
            medications: [{ medicineName: '', dosage: '', frequency: '', duration: '', notes: '' }]
        }
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'medications'
    });

    useEffect(() => {
        const fetchAppointment = async () => {
            try {
                const data = await appointmentService.getAppointment(appointmentId);
                setAppointment(data);
            } catch (error) {
                toast.error(error || 'Failed to load appointment');
                navigate('/doctor/appointments');
            }
        };
        fetchAppointment();
    }, [appointmentId, navigate]);

    const onSubmit = async (data) => {
        setLoading(true);
        try {
            await prescriptionService.createPrescription({
                appointmentId: parseInt(appointmentId),
                diagnosis: data.diagnosis,
                instructions: data.instructions,
                medications: data.medications
            });
            toast.success('Prescription created successfully!');
            navigate('/doctor/appointments');
        } catch (error) {
            toast.error(error || 'Failed to create prescription');
        } finally {
            setLoading(false);
        }
    };

    if (!appointment) {
        return <div className="page-container">Loading...</div>;
    }

  return (
    <div className="page-container max-w-4xl">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">Create Prescription</h1>
        <p className="text-gray-600 mt-2">
          Patient: {appointment.patientName} | Date: {new Date(appointment.appointmentDate).toLocaleDateString()}
        </p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="card">
          <h2 className="text-xl font-bold mb-4">Diagnosis & Instructions</h2>
          
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Diagnosis
            </label>
            <textarea
              {...register('diagnosis', { required: 'Diagnosis is required' })}
              rows="3"
              className="input-field"
              placeholder="Enter diagnosis..."
            />
            {errors.diagnosis && (
              <p className="text-red-600 text-sm mt-1">{errors.diagnosis.message}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Instructions
            </label>
            <textarea
              {...register('instructions')}
              rows="3"
              className="input-field"
              placeholder="General instructions for the patient..."
            />
          </div>
        </div>

        <div className="card">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-bold">Medications</h2>
            <Button
              type="button"
              variant="outline"
              onClick={() => append({ medicineName: '', dosage: '', frequency: '', duration: '', notes: '' })}
            >
              <Plus size={18} />
              Add Medication
            </Button>
          </div>

          <div className="space-y-4">
            {fields.map((field, index) => (
              <div key={field.id} className="border border-gray-200 rounded-lg p-4">
                <div className="flex justify-between items-start mb-3">
                  <h3 className="font-medium">Medication {index + 1}</h3>
                  {fields.length > 1 && (
                    <button
                      type="button"
                      onClick={() => remove(index)}
                      className="text-red-600 hover:text-red-700"
                    >
                      <Trash2 size={18} />
                    </button>
                  )}
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="md:col-span-2">
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Medicine Name *
                    </label>
                    <input
                      {...register(`medications.${index}.medicineName`, { 
                        required: 'Medicine name is required' 
                      })}
                      className="input-field"
                      placeholder="e.g., Amoxicillin"
                    />
                    {errors.medications?.[index]?.medicineName && (
                      <p className="text-red-600 text-sm mt-1">
                        {errors.medications[index].medicineName.message}
                      </p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Dosage
                    </label>
                    <input
                      {...register(`medications.${index}.dosage`)}
                      className="input-field"
                      placeholder="e.g., 500mg"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Frequency
                    </label>
                    <input
                      {...register(`medications.${index}.frequency`)}
                      className="input-field"
                      placeholder="e.g., Twice daily"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Duration
                    </label>
                    <input
                      {...register(`medications.${index}.duration`)}
                      className="input-field"
                      placeholder="e.g., 7 days"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Notes
                    </label>
                    <input
                      {...register(`medications.${index}.notes`)}
                      className="input-field"
                      placeholder="e.g., Take with food"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="flex gap-4">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate('/doctor/appointments')}
            className="flex-1"
          >
            Cancel
          </Button>
          <Button
            type="submit"
            variant="primary"
            loading={loading}
            className="flex-1"
          >
            Create Prescription
          </Button>
        </div>
      </form>
    </div>
  );
};

export default CreatePrescription;