import { useState } from 'react';
import { format } from 'date-fns';
import { Calendar, Clock, XCircle } from 'lucide-react';
import { toast } from 'react-toastify';
import appointmentService from '../../services/appointmentService';
import Button from '../common/Button';

const AppointmentCard = ({ appointment, onUpdate }) => {
  const [loading, setLoading] = useState(false);
  const [reason, setReason] = useState('');
  const [open, setOpen] = useState(false);

  const canCancel =
    appointment.status === 'Scheduled' &&
    new Date(appointment.appointmentDate) > new Date();

  const cancelAppointment = async () => {
    if (!reason.trim()) {
      toast.error('Reason is required');
      return;
    }

    setLoading(true);
    try {
      await appointmentService.cancelAppointment(appointment.appointmentId, reason);
      toast.success('Appointment cancelled successfully');
      setOpen(false);
      onUpdate?.();
    } catch {
      toast.error('Failed to cancel appointment');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="card">
        <h3 className="font-semibold">Dr. {appointment.doctorName}</h3>

        <div className="mt-2 text-sm">
          <Calendar size={16} />{' '}
          {format(new Date(appointment.appointmentDate), 'MMM dd, yyyy')}
        </div>

        <div className="text-sm">
          <Clock size={16} /> {appointment.appointmentTime?.slice(0, 5)}
        </div>

        {canCancel && (
          <Button variant="danger" onClick={() => setOpen(true)} className="w-full mt-3">
            <XCircle size={18} /> Cancel
          </Button>
        )}
      </div>

      {open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center">
          <div className="bg-white p-6 rounded w-full max-w-md">
            <textarea
              className="w-full border p-2 mb-4"
              placeholder="Reason"
              value={reason}
              onChange={(e) => setReason(e.target.value)}
            />
            <div className="flex gap-2">
              <Button variant="outline" onClick={() => setOpen(false)}>Close</Button>
              <Button loading={loading} variant="danger" onClick={cancelAppointment}>
                Confirm
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default AppointmentCard;
