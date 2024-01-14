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

const RegisteredNumberField = ({ name, labelText, register, disabled }) => (
  <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
    <b>{labelText}</b>
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

export { RegisteredInput, RegisteredSelect, RegisteredNumberField };
