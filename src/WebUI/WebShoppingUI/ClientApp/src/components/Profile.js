import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { ApiEndpoints } from "../api/ApiEndpoints";
import http from "../utils/Http";
import {
  RegisteredDateField,
  RegisteredEmailField,
  RegisteredTextField,
} from "./FormElements";
import "../site.scss";

export function Profile() {
  const [originalProfile, setOriginalProfile] = useState(null);
  const {
    register,
    reset: resetForm,
    handleSubmit,
  } = useForm({ defaultValues: originalProfile });
  const [loading, setLoading] = useState(true);
  const [isEditingInfo, setIsEditingInfo] = useState(false);
  const [isEditingEmail, setIsEditingEmail] = useState(false);

  const onSubmit = async (data) => {
    if (isEditingInfo) {
      const response = await http.patchAsync(
        ApiEndpoints.userPersonalInfo(data.id),
        {
          emailAddress: data.emailAddress,
          firstName: data.firstName,
          lastName: data.lastName,
          phoneNumber: data.phoneNumber,
          dateOfBirth: data.dateOfBirth,
          gender: data.gender,
        }
      );
      if (response.ok) {
        alert("Personal info updated.");
        exitEdit(data);
      } else {
        var respBody = await response.json();
        alert(respBody.error);
        exitEdit();
      }
    } else if (isEditingEmail) {
      const response = await http.patchAsync(
        ApiEndpoints.userEmailAddress(data.id),
        data.emailAddress
      );
      if (response.ok) {
        alert("Email address changed.");
        exitEdit(data);
      } else {
        var respBody = await response.json();
        alert(respBody.error);
        exitEdit();
      }
    }
  };

  const exitEdit = (newFormData) => {
    setIsEditingInfo(false);
    setIsEditingEmail(false);
    if (newFormData) setOriginalProfile(newFormData);
    else resetForm(originalProfile);
  };

  useEffect(() => {
    const getProfileAsync = async () => {
      setLoading(true);
      const response = await fetch(ApiEndpoints.profile("SPER-1"));
      const data = await response.json();
      setOriginalProfile(data);
      setLoading(false);
    };
    getProfileAsync();
  }, []);

  useEffect(() => {
    resetForm(originalProfile);
  }, [originalProfile]);

  return (
    <>
      <h1>Profile</h1>
      <br />
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="d-flex mb-4">
            <RegisteredTextField
              name="id"
              labelText="Customer ID"
              register={register}
              disabled={true}
            />
            <RegisteredEmailField
              name="emailAddress"
              labelText="Email address"
              register={register}
              disabled={!isEditingEmail}
            />
          </div>
          <div className="d-flex mb-4">
            <RegisteredTextField
              name="firstName"
              labelText="First name"
              register={register}
              disabled={!isEditingInfo}
            />
            <RegisteredTextField
              name="lastName"
              labelText="Last name"
              register={register}
              disabled={!isEditingInfo}
            />
          </div>
          <div className="d-flex">
            <RegisteredTextField
              name="phoneNumber"
              labelText="Phone number"
              register={register}
              disabled={!isEditingInfo}
            />
            <RegisteredDateField
              name="dateOfBirth"
              labelText="Date of birth"
              register={register}
              disabled={!isEditingInfo}
            />
            <div className="me-4">
              <span className="form-label semi-bold">Gender</span>
              <select className="form-select mt-2" disabled={!isEditingInfo}>
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </select>
            </div>
          </div>
          <br />
          <div className="d-flex">
            {isEditingInfo || isEditingEmail ? (
              <div className="mt-2">
                <button
                  key="submit"
                  type="submit"
                  className="btn baz-btn-secondary me-4 rounded-pill px-4"
                >
                  Update
                </button>
                <span
                  onClick={() => {
                    exitEdit();
                  }}
                  className="btn btn-secondary rounded-pill px-4"
                >
                  Cancel
                </span>
              </div>
            ) : (
              <div className="mt-2">
                <span
                  onClick={() => setIsEditingEmail(true)}
                  className="btn btn-danger me-4 rounded-pill px-4"
                >
                  Change email address
                </span>
                <span
                  onClick={() => setIsEditingInfo(true)}
                  className="btn baz-btn-primary rounded-pill px-4"
                >
                  Edit personal info
                </span>
              </div>
            )}
          </div>
        </form>
      )}
    </>
  );
}
