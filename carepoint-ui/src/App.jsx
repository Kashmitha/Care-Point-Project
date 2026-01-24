import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { AuthProvider } from './context/AuthContext.jsx';
import ProtectedRoute from './components/common/ProtectedRoute';
import Navbar from './components/layout/Navbar';
import Footer from './components/layout/Footer';

// Pages
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PatientDashboard from './pages/patient/PatientDashboard';
import BookAppointmentPage from './pages/patient/BookAppointmentPage';
import DoctorDashboard from './pages/doctor/DoctorDashboard';
import AdminDashboard from './pages/admin/AdminDashboard';
import ManageDoctors from './pages/admin/ManageDoctors';

function App() {
  return (
    <Router>
      <AuthProvider>
        <div className="flex flex-col min-h-screen">
          <Navbar />
          <main className="flex-grow">
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />

              {/* Patient Routes */}
              <Route 
                path="/patient/dashboard"
                element={
                  <ProtectedRoute allowedRoles={['Patient']}>
                    <PatientDashboard />
                  </ProtectedRoute>
                }
              />
              <Route 
                path="/patient/book-appointment"
                element={
                  <ProtectedRoute allowedRoles={['Patient']}>
                    <BookAppointmentPage />
                  </ProtectedRoute>
                }
              />

              {/* Doctor Routes */}
              <Route 
                path="/doctor/dashboard"
                element={
                  <ProtectedRoute allowedRoles={['Doctor']}>
                    <DoctorDashboard />
                  </ProtectedRoute>
                }
              />

              {/* Admin Routes */}
              <Route 
                path="/admin/dashboard"
                element={
                  <ProtectedRoute allowedRoles={['Admin']}>
                    <AdminDashboard />
                  </ProtectedRoute>
                }
              />

              <Route 
                path="/admin/doctors"
                element={
                  <ProtectedRoute allowedRoles={['Admin']}>
                    <ManageDoctors />
                  </ProtectedRoute>
                }
              />

              {/* Unauthorized */}
              <Route 
                path="/unauthorized"
                element={
                  <div className="page-container text-center py-16">
                    <h1 className="text-4xl font-bold text-red-600 mb-4">Access Denied</h1>
                    <p className="text-gray-600">You don't have permission to access this page.</p>
                  </div>
                }
              />

              {/* 404 */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </main>
          <Footer />
        </div>

        <ToastContainer 
          position="top-right"
          autoClose={3000}
          hideProgressBar={false}
          newestOnTop
          closeOnClick
          pauseOnHover
          theme="light"
        />
      </AuthProvider>
    </Router>
  );
}

export default App;