import { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import prescriptionService from '../../services/prescriptionService';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { Pill, Calendar, User } from 'lucide-react';

const PrescriptionPage = () => {
    const [prescriptions, setPrescriptions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedPrescription, setSelectedPrescription] = useState(null);

    useEffect(() => {
        const fetchPrescriptions = async () => {
            try {
                const data = await prescriptionService.getPatientPrescriptions();
                setPrescriptions(data);
            } catch (error) {
                toast.error(error || 'Failed to load prescriptions');
            } finally {
                setLoading(false);
            }
        };

        fetchPrescriptions();
    }, []);

    if (loading) {
        return (
            <div className="page-container flex justify-center items-center min-h-screen">
                <LoadingSpinner size="large" />
            </div>
        );
    }

  return (
    <div className="page-container">
      <h1 className="text-3xl font-bold mb-8">My Prescriptions</h1>

      {prescriptions.length === 0 ? (
        <div className="card text-center py-12">
          <Pill className="mx-auto text-gray-300 mb-4" size={64} />
          <p className="text-gray-600 text-lg">No prescriptions yet</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {prescriptions.map((prescription) => (
            <div
              key={prescription.prescriptionId}
              className="card cursor-pointer hover:shadow-lg transition-shadow"
              onClick={() => setSelectedPrescription(prescription)}
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="font-bold text-lg text-gray-900">
                    Dr. {prescription.doctorName}
                  </h3>
                  <p className="text-sm text-gray-600 flex items-center gap-1 mt-1">
                    <Calendar size={14} />
                    {new Date(prescription.createdAt).toLocaleDateString()}
                  </p>
                </div>
                <span className="bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm font-medium">
                  {prescription.medications.length} Medicine(s)
                </span>
              </div>

              {prescription.diagnosis && (
                <div className="mb-3 p-3 bg-blue-50 rounded-lg">
                  <p className="text-sm font-medium text-blue-900">Diagnosis</p>
                  <p className="text-sm text-blue-800">{prescription.diagnosis}</p>
                </div>
              )}

              <div>
                <p className="text-sm font-medium text-gray-700 mb-2">Medications:</p>
                <ul className="space-y-1">
                  {prescription.medications.slice(0, 3).map((med) => (
                    <li key={med.medicationId} className="text-sm text-gray-600">
                      • {med.medicineName} {med.dosage && `- ${med.dosage}`}
                    </li>
                  ))}
                  {prescription.medications.length > 3 && (
                    <li className="text-sm text-primary-600 font-medium">
                      +{prescription.medications.length - 3} more medications
                    </li>
                  )}
                </ul>
              </div>

              <button className="mt-4 text-primary-600 hover:text-primary-700 text-sm font-medium">
                View Full Details →
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Prescription Detail Modal */}
      {selectedPrescription && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
          onClick={() => setSelectedPrescription(null)}
        >
          <div
            className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto"
            onClick={(e) => e.stopPropagation()}
          >
            <h2 className="text-2xl font-bold mb-4">Prescription Details</h2>

            <div className="mb-6">
              <div className="flex items-center gap-2 text-gray-700 mb-2">
                <User size={18} />
                <span className="font-medium">Dr. {selectedPrescription.doctorName}</span>
              </div>
              <div className="flex items-center gap-2 text-gray-600">
                <Calendar size={18} />
                <span>{new Date(selectedPrescription.createdAt).toLocaleDateString()}</span>
              </div>
            </div>

            {selectedPrescription.diagnosis && (
              <div className="mb-6 p-4 bg-blue-50 rounded-lg">
                <h3 className="font-semibold text-blue-900 mb-2">Diagnosis</h3>
                <p className="text-blue-800">{selectedPrescription.diagnosis}</p>
              </div>
            )}

            {selectedPrescription.instructions && (
              <div className="mb-6 p-4 bg-green-50 rounded-lg">
                <h3 className="font-semibold text-green-900 mb-2">Instructions</h3>
                <p className="text-green-800">{selectedPrescription.instructions}</p>
              </div>
            )}

            <div className="mb-6">
              <h3 className="font-semibold text-gray-900 mb-3">Prescribed Medications</h3>
              <div className="space-y-3">
                {selectedPrescription.medications.map((med, index) => (
                  <div key={med.medicationId} className="border border-gray-200 rounded-lg p-4">
                    <h4 className="font-medium text-gray-900 mb-2">
                      {index + 1}. {med.medicineName}
                    </h4>
                    <div className="grid grid-cols-2 gap-2 text-sm">
                      {med.dosage && (
                        <p className="text-gray-600">
                          <span className="font-medium">Dosage:</span> {med.dosage}
                        </p>
                      )}
                      {med.frequency && (
                        <p className="text-gray-600">
                          <span className="font-medium">Frequency:</span> {med.frequency}
                        </p>
                      )}
                      {med.duration && (
                        <p className="text-gray-600">
                          <span className="font-medium">Duration:</span> {med.duration}
                        </p>
                      )}
                      {med.notes && (
                        <p className="text-gray-600 col-span-2">
                          <span className="font-medium">Notes:</span> {med.notes}
                        </p>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <button
              onClick={() => setSelectedPrescription(null)}
              className="btn-primary w-full"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default PrescriptionPage;