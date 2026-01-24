import { useState, useEffect } from 'react';
import adminService from '../../services/adminService';
import { Users, Calendar, CheckCircle, Clock } from 'lucide-react';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { toast } from 'react-toastify';
import { Link } from 'react-router-dom';

const AdminDashboard = () => {
    const [loading, setLoading] = useState(true);
    const [stats, setStats] = useState(null);

    useEffect(() => {
        fetchStats();
    }, []);

    const fetchStats = async () => {
        try {
            const data = await adminService.getDashboardStats();
            setStats(data);
        } catch (error) {
            toast.error(error || 'Failed to load statistics');
        } finally {
            setLoading(false);
        }
    };

    if (loading) 
    {
        return (
            <div className="page-container flex justify-center items-center min-h-screen">
                <LoadingSpinner size="large" />
            </div>
        );
    }

  return (
    <div className="page-container">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Admin Dashboard</h1>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Users</p>
              <p className="text-3xl font-bold">{stats.totalUsers}</p>
            </div>
            <Users className="w-12 h-12 text-primary-600" />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Doctors</p>
              <p className="text-3xl font-bold">{stats.totalDoctors}</p>
            </div>
            <Users className="w-12 h-12 text-secondary-600" />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Pending Approvals</p>
              <p className="text-3xl font-bold">{stats.pendingDoctors}</p>
            </div>
            <Clock className="w-12 h-12 text-yellow-600" />
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Total Appointments</p>
              <p className="text-3xl font-bold">{stats.totalAppointments}</p>
            </div>
            <Calendar className="w-12 h-12 text-green-600" />
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <Link to="/admin/doctors" className="card hover:shadow-lg transition-shadow">
          <h3 className="text-xl font-semibold mb-2">Pending Doctor Approvals</h3>
          <p className="text-gray-600 mb-4">{stats.pendingDoctors} doctors waiting for approval</p>
          <button className="btn-primary">Review Now</button>
        </Link>

        <div className="card">
          <h3 className="text-xl font-semibold mb-2">System Overview</h3>
          <div className="space-y-2">
            <p className="text-sm">Patients: {stats.totalPatients}</p>
            <p className="text-sm">Approved Doctors: {stats.approvedDoctors}</p>
            <p className="text-sm">Today's Appointments: {stats.todayAppointments}</p>
          </div>
        </div>
      </div>

      {/* Specialty Stats */}
      <div className="card">
        <h2 className="text-2xl font-bold mb-4">Specialties Overview</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {stats.specialtyStats.map((specialty, index) => (
            <div key={index} className="border border-gray-200 rounded-lg p-4">
              <h3 className="font-semibold text-lg">{specialty.specialtyName}</h3>
              <p className="text-sm text-gray-600">Doctors: {specialty.doctorCount}</p>
              <p className="text-sm text-gray-600">Appointments: {specialty.appointmentCount}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );   
};

export default AdminDashboard;