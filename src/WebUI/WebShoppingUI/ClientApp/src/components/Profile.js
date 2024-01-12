import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { ApiEndpoints } from "../backend/ApiEndpoints";
import http from "../utils/Http";

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
      await http.patchAsync(
        ApiEndpoints.userPersonalInfo(data.id),
        {
          emailAddress: data.emailAddress,
          firstName: data.firstName,
          lastName: data.lastName,
          phoneNumber: data.phoneNumber,
          dateOfBirth: data.dateOfBirth,
          gender: data.gender,
        },
        () => {
          alert("Personal info updated.");
          exitEdit(data);
        },
        async (r) => {
          var respBody = await r.json();
          alert(respBody.error);
          exitEdit();
        }
      );
    } else if (isEditingEmail) {
      await http.patchAsync(
        ApiEndpoints.userEmailAddress(data.id),
        data.emailAddress,
        () => {
          alert("Email address changed.");
          exitEdit(data);
        },
        async (r) => {
          var respBody = await r.json();
          alert(respBody.error);
          exitEdit();
        }
      );
    }
  };

  const exitEdit = (newFormData) => {
    setIsEditingInfo(false);
    setIsEditingEmail(false);
    if (newFormData) setOriginalProfile(newFormData);
    else resetForm(originalProfile);
  };

  const RegisteredInput = ({ name, type = "text", register, disabled }) => {
    return (
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
    );
  };

  const RegisteredSelect = ({ name, register, disabled }) => {
    return (
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
    );
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
            <div>
              <b>Customer ID</b>
              <RegisteredInput name="id" register={register} disabled={true} />
            </div>
            <div style={{ margin: "0 0 0 1.5rem" }}>
              <b>Email address</b>
              <RegisteredInput
                name="emailAddress"
                register={register}
                disabled={!isEditingEmail}
              />
            </div>
          </div>
          <div style={{ display: "flex" }}>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>First name</b>
              <RegisteredInput
                name="firstName"
                register={register}
                disabled={!isEditingInfo}
              />
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Last name</b>
              <RegisteredInput
                name="lastName"
                register={register}
                disabled={!isEditingInfo}
              />
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Date of birth</b>
              <RegisteredInput
                name="dateOfBirth"
                type="date"
                register={register}
                disabled={!isEditingInfo}
              />
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Gender</b>
              <RegisteredSelect
                name="gender"
                register={register}
                disabled={!isEditingInfo}
              />
            </div>
          </div>
          <div>
            <div>
              <b>Phone number</b>
              <RegisteredInput
                name="phoneNumber"
                register={register}
                disabled={!isEditingInfo}
              />
            </div>
            <br />
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
