import { useState } from 'react';
import { format } from 'date-fns';
import { Calendar, Clock, User, XCircle } from 'lucide-react';
import { toast } from 'react-toastify';
import appointmentService from '../../services/appointmentService';
import Button from '../common/Button';

const AppointmentCard = ({ appointment, onUpdate }) => {
  const [cancelling, setCancelling] = useState(false);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [cancelReason, setCancelReason] = useState('');

  const handleCancel = async () => {
    if (!cancelReason.trim()) {
      toast.error('Please provide a cancellation reason');
      return;
    }

    setCancelling(true);
    try {
      await appointmentService.cancelAppointment(appointment.appointmentId, cancelReason);
      toast.success('Appointment cancelled successfully');
      setShowCancelDialog(false);
      if (onUpdate) onUpdate();
    } catch (error) {
      toast.error(error || 'Failed to cancel appointment');
    } finally {
      setCancelling(false);
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Scheduled':
        return 'bg-blue-100 text-blue-800';
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const canCancel = appointment.status === 'Scheduled' && 
    new Date(appointment.appointmentDate) > new Date();

  return (
    <>
      <div className="card">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h3 className="text-lg font-semibold text-gray-900">
              Dr. {appointment.doctorName}
            </h3>
            <p className="text-sm text-gray-600">{appointment.specialtyName}</p>
          </div>
          <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(appointment.status)}`}>
            {appointment.status}
          </span>
        </div>

        <div className="space-y-2 mb-4">
          <div className="flex items-center gap-2 text-gray-700">
            <Calendar size={18} />
            <span>{format(new Date(appointment.appointmentDate), 'MMMM dd, yyyy')}</span>
          </div>
          <div className="flex items-center gap-2 text-gray-700">
            <Clock size={18} />
            <span>{appointment.appointmentTime.substring(0, 5)}</span>
          </div>
          {appointment.reasonForVisit && (
            <div className="mt-2 p-2 bg-gray-50 rounded">
              <p className="text-sm text-gray-600">
                <strong>Reason:</strong> {appointment.reasonForVisit}
              </p>
            </div>
          )}
        </div>

        {canCancel && (
          <Button
            variant="danger"
            onClick={() => setShowCancelDialog(true)}
            className="w-full"
          >
            <XCircle size={18} />
            Cancel Appointment
          </Button>
        )}
      </div>

      {/* Cancel Dialog */}
      {showCancelDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-md w-full">
            <h3 className="text-xl font-bold mb-4">Cancel Appointment</h3>
            <p className="text-gray-600 mb-4">
              Please provide a reason for cancellation:
            </p>
            <textarea
              value={cancelReason}
              onChange={(e) => setCancelReason(e.target.value)}
              className="w-full border border-gray-300 rounded-lg p-3 mb-4"
              rows="4"
              placeholder="Enter cancellation reason..."
            />
            <div className="flex gap-3">
              <Button
                variant="outline"
                onClick={() => setShowCancelDialog(false)}
                className="flex-1"
              >
                Keep Appointment
              </Button>
              <Button
                variant="danger"
                onClick={handleCancel}
                loading={cancelling}
                className="flex-1"
              >
                Confirm Cancellation
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default AppointmentCard;
