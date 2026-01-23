import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from  'react-toastify';
import { useAuth } from '../hooks/useAuth';
import Input from '../components/common/Input';
import Button from '../components/common/Button';

const loginSchema = z.object ({
    email: z.string().email('Invalid email address'),
    password: z.string().min(6, 'Password must be at least 6 characters')
});

const LoginPage = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const { register, handleSubmit, formState: { errors } } = useForm ({
        resolver: zodResolver(loginSchema)
    });

    const onSubmit = async (data) => {
        setLoading(true);
        try {
            const response = await login(data);
            toast.success('Login successful!');

            switch (response.role) {
                case 'Patient': 
                    navigate('/doctor/dashboard');
                    break;
                
                case 'Doctor':
                    navigate('/doctor/dashboard');
                    break;
                
                case 'Admin':
                    navigate('/admin/dashboard');
                    break;
                
                default:
                    navigate('/');
            }
        } catch(error) {
            toast.error(error || 'Login failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-secondary-50 py-12 px-4">
            <div className="max-w-md w-full">
                <div className="card">
                    <div className="text-center mb-8">
                        <div className="w-16 h-16 bg-primary-600 rounded-full flex items-center justify-center mx-auto mb-4">
                        <span className="text-white font-bold text-2xl">C</span>
                        </div>
                        <h2 className="text-3xl font-bold text-gray-900">Welcome Back</h2>
                        <p className="text-gray-600 mt-2">Sign in to your CarePoint account</p>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                        <Input
                        {...register('email')}
                        label="Email Address"
                        type="email"
                        placeholder="your.email@example.com"
                        error={errors.email?.message}
                        />

                        <Input
                        {...register('password')}
                        label="Password"
                        type="password"
                        placeholder="Enter your password"
                        error={errors.password?.message}
                        />

                        <Button type="submit" variant="primary" loading={loading} className="w-full">
                        Sign In
                        </Button>
                    </form>

                    <div className="mt-6 text-center">
                        <p className="text-sm text-gray-600">
                        Don't have an account?{' '}
                        <Link to="/register" className="text-primary-600 hover:text-primary-700 font-medium">
                            Sign up now
                        </Link>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default LoginPage;