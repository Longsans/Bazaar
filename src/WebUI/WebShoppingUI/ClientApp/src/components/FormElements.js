import React, { Children, useEffect } from "react";
import { useForm } from "react-hook-form";

const RegisteredInput = ({
  name,
  labelText,
  register,
  type,
  options = {},
  disabled = false,
}) => {
  return (
    <div className="me-4">
      <span className="form-label semi-bold">{labelText}</span>
      <input
        type={type}
        {...register(name, { required: true, ...options })}
        className="form-control mt-2"
        disabled={disabled}
      />
    </div>
  );
};

const RegisteredTextField = ({
  name,
  labelText,
  register,
  disabled = false,
}) => (
  <RegisteredInput
    name={name}
    labelText={labelText}
    register={register}
    type="text"
    disabled={disabled}
  />
);

const RegisteredNumberField = ({
  name,
  labelText,
  register,
  disabled = false,
}) => (
  <RegisteredInput
    name={name}
    labelText={labelText}
    register={register}
    type="number"
    options={{ valueAsNumber: true, validate: (value) => value > 0 }}
    disabled={disabled}
  />
);

const RegisteredDateField = ({
  name,
  labelText,
  register,
  disabled = false,
}) => (
  <RegisteredInput
    name={name}
    labelText={labelText}
    register={register}
    type="date"
    disabled={disabled}
  />
);

const RegisteredEmailField = ({
  name,
  labelText,
  register,
  disabled = false,
}) => (
  <RegisteredInput
    name={name}
    labelText={labelText}
    register={register}
    type="email"
    disabled={disabled}
  />
);

// children here is a function that takes in the register function and renders all components in the form
function RegisteredForm({ children, onSubmit, defaultValues = null }) {
  const { register, handleSubmit, reset } = useForm({ defaultValues });

  return <form onSubmit={handleSubmit(onSubmit)}>{children(register)}</form>;
}

export {
  RegisteredTextField,
  RegisteredNumberField,
  RegisteredDateField,
  RegisteredEmailField,
  RegisteredForm,
  RegisteredInput,
};
