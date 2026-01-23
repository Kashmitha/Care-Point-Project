import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'react-toastify';
import { useAuth } from '../../hooks/useAuth';
import Input from '../components/common/Input';
import Select from '../components/common/Select';
import Button from '../components/common/Button';

const registerSchema = z.object ({
    email: z.string().email('Invalid email address'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
    confirmPassword: z.string(),
    firstName: z.string().min(2, 'First name must be at least 2 characters'),
    lastName: z.string().min(2, 'Last name must be at least 2 characters'),
    phoneNumber: z.string().optional(),
    dateOfBirth: z.string().optional(),
    gender: z.string().optional(),
    role: z.enum(['Patient', 'Doctor']),
    licenseNumber: z.string().optional(),
    specialtyId: z.string().optional(),
    yearsOfExperience: z.string().optional(),
    qualification: z.string().optional(),
    consultationFee: z.string().optional()
}).refine((data) => data.password === data.confirmPassword, {
    message: "Password don't match",
    path: ['confirmPassword']
});

const RegisterPage = () => {
    const { register: registerUser } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const { register, handleSubmit, formState: { errors }, watch } = useForm({
        resolver: zodResolver(registerSchema),
        defaultValues: { role: 'Patient' }
    });

    const watchRole = watch('role');

    const specialties = [
        { value: '1', label: 'Cardiology' },
        { value: '2', label: 'Neurology' },
        { value: '3', label: 'Pediatrics' },
        { value: '4', label: 'Orthopedics' },
        { value: '5', label: 'Dermatology' }
    ];

    const genderOptions = [
        { value: 'Male', label: 'Male' },
        { value: 'Female', label: 'Female' },
        { value: 'Other', label: 'Other' }
    ];

    const onSubmit = async (data) => {
        setLoading(true);
        try {
            const formattedData = {
                ...data,
                specialtyId: data.specialtyId ? parseInt(data.specialtyId) : null,
                yearsOfExperience: data.yearsOfExperience ? parseInt(data.yearsOfExperience) : null,
                consultationFee: data.consultationFee ? parseFloat(data.consultationFee) : null
            };

            await registerUser(formattedData);
            toast.success('Registration successful!');

            if(data.role === 'Patient') {
                navigate('/patient/dashboard');
            } else {
                toast.info('Your doctor account is pending approval');
                navigate('/doctor/dashboard');
            }
        } catch (error) {
            toast.error(error || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-primary-50 to-secondary-50 py-12 px-4">
        <div className="max-w-3xl mx-auto">
            <div className="card">
            <div className="text-center mb-8">
                <h2 className="text-3xl font-bold text-gray-900">Create Account</h2>
                <p className="text-gray-600 mt-2">Join CarePoint today</p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                {/* Role Selection */}
                <div className="bg-gray-50 p-4 rounded-lg">
                <Select
                    {...register('role')}
                    label="I am registering as"
                    options={[
                    { value: 'Patient', label: 'Patient' },
                    { value: 'Doctor', label: 'Doctor' }
                    ]}
                    error={errors.role?.message}
                />
                </div>

                {/* Basic Information */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                    {...register('firstName')}
                    label="First Name"
                    placeholder="John"
                    error={errors.firstName?.message}
                />
                <Input
                    {...register('lastName')}
                    label="Last Name"
                    placeholder="Doe"
                    error={errors.lastName?.message}
                />
                </div>

                <Input
                {...register('email')}
                label="Email Address"
                type="email"
                placeholder="your.email@example.com"
                error={errors.email?.message}
                />

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                    {...register('password')}
                    label="Password"
                    type="password"
                    placeholder="Minimum 6 characters"
                    error={errors.password?.message}
                />
                <Input
                    {...register('confirmPassword')}
                    label="Confirm Password"
                    type="password"
                    placeholder="Re-enter password"
                    error={errors.confirmPassword?.message}
                />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                    {...register('phoneNumber')}
                    label="Phone Number"
                    type="tel"
                    placeholder="+1 (555) 123-4567"
                />
                <Input
                    {...register('dateOfBirth')}
                    label="Date of Birth"
                    type="date"
                />
                </div>

                <Select
                {...register('gender')}
                label="Gender"
                options={genderOptions}
                />

                {/* Doctor-Specific Fields */}
                {watchRole === 'Doctor' && (
                <div className="border-t pt-6 mt-6">
                    <h3 className="text-xl font-semibold text-gray-900 mb-4">
                    Professional Information
                    </h3>

                    <Input
                    {...register('licenseNumber')}
                    label="Medical License Number"
                    placeholder="MD123456"
                    error={errors.licenseNumber?.message}
                    />

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <Select
                        {...register('specialtyId')}
                        label="Specialty"
                        options={specialties}
                    />
                    <Input
                        {...register('yearsOfExperience')}
                        label="Years of Experience"
                        type="number"
                        placeholder="5"
                    />
                    </div>

                    <Input
                    {...register('qualification')}
                    label="Qualification"
                    placeholder="MD, MBBS"
                    />

                    <Input
                    {...register('consultationFee')}
                    label="Consultation Fee ($)"
                    type="number"
                    step="0.01"
                    placeholder="100.00"
                    />
                </div>
                )}

                <Button type="submit" variant="primary" loading={loading} className="w-full">
                Create Account
                </Button>
            </form>

            <div className="mt-6 text-center">
                <p className="text-sm text-gray-600">
                Already have an account?{' '}
                <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
                    Sign in
                </Link>
                </p>
            </div>
            </div>
        </div>
        </div>
    )
}

export default RegisterPage;