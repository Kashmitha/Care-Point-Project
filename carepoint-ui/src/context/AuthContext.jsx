import { useState } from 'react';
import PropTypes from 'prop-types';  // added PropTypes
import authService from '../services/authService';
import AuthContext from './authContext';

/**
 * Authentication context for managing user state globally.
 */

/**
 * Auth Provider component.
 * Wraps the app to provide authentication state globally.
 */
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => authService.getCurrentUser());
  const [loading] = useState(false);

  // Login function
  const login = async (credentials) => {
    const response = await authService.login(credentials);
    setUser(response);
    return response;
  };

  // Register function
  const register = async (userData) => {
    const response = await authService.register(userData);
    setUser(response);
    return response;
  };

  // Logout function
  const logout = () => {
    authService.logout();
    setUser(null);
  };

  // Context value
  const value = {
    user,
    login,
    register,
    logout,
    isAuthenticated: !!user,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

// PropTypes for type checking
AuthProvider.propTypes = {
  children: PropTypes.node.isRequired
};

// Default export for convenience
export default AuthProvider;