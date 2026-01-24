import { Link } from 'react-router-dom';
import { Calendar, Users, FileText, Shield } from 'lucide-react';

const HomePage = () => {
    const features = [
        {
            icon: <Calendar className="w-12 h-12 text-primary-600" />,
            title: 'Easy Appointment Booking',
            description: 'Book appointments with your preferred doctors in just a few clicks'
        },
        {
            icon: <Users className="w-12 h-12 text-primary-600" />,
            title: 'Verified Doctors',
            description: 'All our doctors are verified and licensed healthcare professionals'
        },
        {
            icon: <FileText className="w-12 h-12 text-primary-600" />,
            title: 'Digital Records',
            description: 'Access your medical history and prescriptions anytime'
        },
        {
            icon: <Shield className="w-12 h-12 text-primary-600" />,
            title: 'Secure & Private',
            description: 'Your health data is encrypted and protected'
        }
    ];

    return (
        <div className="min-h-screen">
            {/* Hero Section */}
            <div className="bg-gradient-to-br from-primary-600 to-primary-800 text-white">
                <div className="page-container py-20">
                    <div className="max-w-3xl">
                        <h1 className="text-5xl font-bold mb-6">Your Health, Our Priority</h1>
                        <p className="text-xl mb-8 text-primary-100">
                            CarePoint connects you with trusted healthcare professionals.
                        </p>
                        <div className="flex gap-4">
                            <Link to="/register" className="bg-white text-primary-600 px-8 py-3 rounded-lg font-semibold hover:bg-primary-50">
                                Get Started
                            </Link>
                            <Link to="/login" className="border-2 border-white text-white px-8 py-3 rounded-lg font-semibold hover:bg-white hover:text-primary-600">
                                Sign In
                            </Link>
                        </div>
                    </div>
                </div>
            </div>

            {/* Features */}
            <div className="page-container py-16">
                <h2 className="text-3xl font-bold text-center mb-12">Why Choose CarePoint?</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
                    {features.map((feature, index) => (
                        <div key={index} className="card text-center hover:shadow-lg transition-shadow">
                            <div className="flex justify-center mb-4">{feature.icon}</div>
                            <h3 className="text-xl font-semibold mb-2">{feature.title}</h3>
                            <p className="text-gray-600">{feature.description}</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default HomePage;