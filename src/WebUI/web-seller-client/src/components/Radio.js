import React from "react";

export default function Radio({ name, value, displayText, isDefault }) {
  return (
    <div className="form-check">
      <input
        className="form-check-input"
        type="radio"
        name={name}
        value={value}
        defaultChecked={isDefault}
        id={name + value}
      />
      <label className="form-check-label" htmlFor={name + value}>
        {displayText}
      </label>
    </div>
  );
}
