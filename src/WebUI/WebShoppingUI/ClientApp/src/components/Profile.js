import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { ApiEndpoints } from "../api/ApiEndpoints";
import http from "../utils/Http";
import { RegisteredInput, RegisteredSelect } from "./FormElements";

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
      const response = await fetch(ApiEndpoints.fetchProfile("SPER-1"));
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
          <div style={{ display: "flex" }}>
            <RegisteredInput
              name="id"
              labelText="Customer ID"
              register={register}
              disabled={true}
            />
            <RegisteredInput
              name="emailAddress"
              labelText="Email address"
              register={register}
              disabled={!isEditingEmail}
            />
          </div>
          <div style={{ display: "flex" }}>
            <RegisteredInput
              name="firstName"
              labelText="First name"
              register={register}
              disabled={!isEditingInfo}
            />
            <RegisteredInput
              name="lastName"
              labelText="Last name"
              register={register}
              disabled={!isEditingInfo}
            />
            <RegisteredInput
              name="dateOfBirth"
              labelText="Date of birth"
              type="date"
              register={register}
              disabled={!isEditingInfo}
            />
            <RegisteredSelect
              name="gender"
              labelText="Gender"
              register={register}
              disabled={!isEditingInfo}
            />
          </div>
          <div style={{ display: "flex" }}>
            <RegisteredInput
              name="phoneNumber"
              labelText="Phone number"
              register={register}
              disabled={!isEditingInfo}
            />
          </div>
          <br />
          <div style={{ display: "flex" }}>
            {isEditingInfo || isEditingEmail ? (
              <>
                <button
                  key="submit"
                  type="submit"
                  style={{ margin: "0 1rem 0 0" }}
                >
                  Update
                </button>
                <button
                  key="cancel"
                  type="button"
                  onClick={() => {
                    exitEdit();
                  }}
                >
                  Cancel
                </button>
              </>
            ) : (
              <>
                <button
                  key="edit-info"
                  type="button"
                  onClick={() => setIsEditingInfo(true)}
                  style={{ margin: "0 1.5rem 0 0" }}
                >
                  Edit personal info
                </button>
                <button
                  key="edit-email"
                  type="button"
                  onClick={() => setIsEditingEmail(true)}
                  style={{ margin: "0 1.5rem 0 0" }}
                >
                  Change email address
                </button>
              </>
            )}
          </div>
        </form>
      )}
    </>
  );
}
