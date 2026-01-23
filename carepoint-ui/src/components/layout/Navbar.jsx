import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { User, LogOut, Calendar, Home } from 'lucide-react';

const Navbar = () => {
    const { user, logout, isAuthenticated } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <nav className="bg-white shadow-md sticky top-0 z-50">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between items-center h-16">
                    <Link to="/" className="flex items-center space-x-2">
                        <div className="w-10 h-10 bg-primary-600 rounded-lg flex items-center justify-center">
                            <span className="text-white font-bold text-xl">C</span>
                        </div>
                        <span className="text-xl font-bold text-gray-900">CarePoint</span>
                    </Link>

                    <div className="flex items-center space-x-4">
                        {isAuthenticated ? (
                            <>
                                <Link to="/" className="text-gray-700 hover:text-primary-600 flex items-center gap-1">
                                    <Home size={18} />
                                    Home
                                </Link>

                                {user?.role === 'Patient' && (
                                    <>
                                        <Link to="/patient/dashboard" className="text-gray-700 hover:text-primary-600 flex items-center gap-1">
                                            <Calendar size={18} />
                                            Dashboard
                                        </Link>
                                        <Link to="/patient/book-appointment" className="text-gray-700 hover:text-primary-600">
                                            Book Appointment
                                        </Link>
                                    </>
                                )}

                                {user?.role === 'Doctor' && (
                                    <>
                                        <Link to="/doctor/dashboard" className="text-gray-700 hover:text-primary-600">
                                            Dashboard
                                        </Link>
                                        <Link to="/doctor/schedule" className="text-gray-700 hover:text-primary-600">
                                            Schedule
                                        </Link>
                                    </>
                                )}

                                {user?.role === 'Admin' && (
                                    <>
                                        <Link to="/admin/dashboard" className="text-gray-700 hover:text-primary-600">
                                            Dashboard
                                        </Link>
                                        <Link to="/admin/doctors" className="text-gray-700 hover:text-primary-600">
                                            Doctors
                                        </Link>
                                    </>
                                )}
                                <div className="flex items-center space-x-3 ml-4 pl-4 border-l border-gray-300">
                                    <div className="flex items-center gap-2">
                                        <User size={18} className="text-gray-600" />
                                        <span className="text-sm font-medium text-gray-700">
                                            {user?.firstName} {user?.lastName}
                                        </span>
                                        <span className="text-xs bg-primary-100 text-primary-700 px-2 py-1 rounded">
                                            {user?.role}
                                        </span>
                                    </div>
                                    <button
                                        onClick={handleLogout}
                                        className="text-gray-700 hover:text-red-600 flex items-center gap-1">
                                        <LogOut size={18} />
                                        Logout
                                    </button>
                                </div>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className="text-gray-700 hover:text-primary-600">
                                    Login
                                </Link>
                                <Link to="/register" className="btn-primary">
                                    Sign Up
                                </Link>
                            </>
                            )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
