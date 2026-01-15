import { Navigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

/**
 * Protected Route wrapper component 
 */
const ProtectedRoute = ({ children, allowedRoles }) => {
    const { user, isAuthenticated } = useAuth();

    if(!isAuthenticated){
        return ;
    }

    if(allowedRoles && !allowedRoles.includes(user.role)) {
        return ;
    }

    return children;
};

export default ProtectedRoute;