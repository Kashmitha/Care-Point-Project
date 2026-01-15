import { forwardRef } from 'react';

const Select = forwardRef(
  (
    {
      label,
      error,
      options = [],
      placeholder = 'Select an option',
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

        <select
          ref={ref}
          className={`px-3 py-2 border rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500
            ${error ? 'border-red-500' : 'border-gray-300'}
            ${className}`}
          {...props}
        >
          <option value="" disabled>
            {placeholder}
          </option>

          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>

        {error && (
          <p className="text-sm text-red-500">
            {error}
          </p>
        )}
      </div>
    );
  }
);

Select.displayName = 'Select';

export default Select;
