import { forwardRef } from 'react';

/**
 * Reusable input component with error handling.
 */

const Input = forwardRef(
  (
    {
      label,
      error,
      type = 'text',
      placeholder,
      className = '',
      ...props
    },
    ref
  ) => {
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label className="text-sm font-medium text-gray-700">
            {label}
          </label>
        )}

        <input
          ref={ref}
          type={type}
          placeholder={placeholder}
          className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500
            ${error ? 'border-red-500' : 'border-gray-300'}
            ${className}`}
          {...props}
        />

        {error && (
          <p className="text-sm text-red-500">
            {error}
          </p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';

export default Input;
