const LoadingSpinner = ({ size = 'medium' }) => {
    const sizeClasses = {
        small: 'w-4 h-4',
        medium: 'w-8 h8',
        large: 'w-12 h-12'
    };

    return (
        <div
            className={`animate-spin rounded-full border-4 border-gray-300 border-t-blue-600 ${sizeClasses[size]}`}
            role="status"
            aria-label="Loading"
        />
    );
};

export default LoadingSpinner;