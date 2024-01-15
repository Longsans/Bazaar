import React, { Children, useEffect } from "react";
import { useForm } from "react-hook-form";

const RegisteredInput = ({
  name,
  labelText,
  type = "text",
  register,
  disabled,
}) => (
  <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
    <b>{labelText}</b>
    <input
      type={type}
      {...register(name, { required: true })}
      disabled={disabled}
      style={{
        display: "block",
        margin: "0.5rem 0px",
        padding: "0 0 0 0.25rem",
        borderRadius: "3px",
      }}
    />
  </div>
);

const RegisteredSelect = ({ name, labelText, register, disabled }) => (
  <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
    <b>{labelText}</b>
    <select
      {...register(name)}
      disabled={disabled}
      style={{
        display: "block",
        margin: "0.5rem 0px",
        backgroundColor: "white",
        borderRadius: "3px",
      }}
    >
      <option value="Male">Male</option>
      <option value="Female">Female</option>
    </select>
  </div>
);

const RegisteredNumberField = ({
  name,
  labelText = null,
  register,
  disabled,
}) => (
  <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
    {labelText && <b>{labelText}</b>}
    <input
      type="text"
      {...register(name, { required: true, pattern: "/^[0-9]*$/" })}
      disabled={disabled}
      style={{
        display: "block",
        margin: "0.5rem 0px",
        padding: "0 0 0 0.25rem",
        borderRadius: "3px",
      }}
    />
  </div>
);

// children here is a function that takes in the register function and renders all components in the form
function RegisteredForm({ children, onSubmit, defaultValues = null }) {
  const { register, handleSubmit, reset } = useForm({ defaultValues });

  return <form onSubmit={handleSubmit(onSubmit)}>{children(register)}</form>;
}

export {
  RegisteredInput,
  RegisteredSelect,
  RegisteredNumberField,
  RegisteredForm,
};
