import { useState, useEffect } from "react";
import { ApiEndpoints } from "../backend/ApiEndpoints";
import http from "../utils/Http";

export function Profile() {
  const [profile, setProfile] = useState(null);
  const [originalProfile, setOriginalProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEditingInfo, setIsEditingInfo] = useState(false);
  const [isEditingEmail, setIsEditingEmail] = useState(false);

  const updatePersonalInfo = async () => {
    await http.patchAsync(
      ApiEndpoints.userPersonalInfo(profile.id),
      {
        emailAddress: profile.emailAddress,
        firstName: profile.firstName,
        lastName: profile.lastName,
        phoneNumber: profile.phoneNumber,
        dateOfBirth: profile.dateOfBirth,
        gender: profile.gender,
      },
      () => {
        alert("Personal info updated.");
        exitEdit(true);
      },
      async (r) => {
        var respBody = await r.json();
        alert(respBody.error);
        exitEdit(false);
      }
    );
  };

  const updateEmailAddress = async () => {
    await http.patchAsync(
      ApiEndpoints.userEmailAddress(profile.id),
      profile.emailAddress,
      () => {
        alert("Email address changed.");
        exitEdit(true);
      },
      async (r) => {
        var respBody = await r.json();
        alert(respBody.error);
        exitEdit(false);
      }
    );
  };

  const startEditInfo = () => {
    setIsEditingInfo(true);
    setOriginalProfile(profile);
  };

  const startEditEmail = () => {
    setIsEditingEmail(true);
    setOriginalProfile(profile);
  };

  const exitEdit = (saveEdit) => {
    setIsEditingInfo(false);
    setIsEditingEmail(false);
    if (!saveEdit) setProfile(originalProfile);
  };

  const patchSetProfile = (key, value) => {
    setProfile({
      ...profile,
      [key]: value,
    });
  };

  useEffect(() => {
    const getProfileAsync = async () => {
      setLoading(true);
      const response = await fetch(ApiEndpoints.fetchProfile("SPER-1"));
      const data = await response.json();
      setProfile(data);
      setLoading(false);
    };
    getProfileAsync();
  }, []);

  return (
    <>
      <h1>Profile</h1>
      <br />
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <>
          <div style={{ display: "flex" }}>
            <div>
              <b>Customer ID</b>
              <input
                type="text"
                value={profile.id}
                disabled
                style={{
                  display: "block",
                  margin: "0.5rem 0px",
                  padding: "0 0 0 0.25rem",
                }}
              />
            </div>
            <div style={{ margin: "0 0 0 1.5rem" }}>
              <b>Email address</b>
              <input
                type="text"
                value={profile.emailAddress}
                onInput={(e) => patchSetProfile("emailAddress", e.target.value)}
                disabled={!isEditingEmail}
                style={{
                  display: "block",
                  margin: "0.5rem 0px",
                  padding: "0 0 0 0.25rem",
                }}
              />
            </div>
          </div>
          <div style={{ display: "flex" }}>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>First name</b>
              <input
                type="text"
                value={profile.firstName}
                onInput={(e) => patchSetProfile("firstName", e.target.value)}
                disabled={!isEditingInfo}
                style={{
                  display: "block",
                  margin: "0.5rem 0px",
                  padding: "0 0 0 0.25rem",
                }}
              ></input>
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Last name</b>
              <input
                type="text"
                value={profile.lastName}
                onInput={(e) => patchSetProfile("lastName", e.target.value)}
                disabled={!isEditingInfo}
                style={{
                  display: "block",
                  margin: "0.5rem 0px",
                  padding: "0 0 0 0.25rem",
                }}
              ></input>
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Date of birth</b>
              <input
                type="date"
                value={profile.dateOfBirth}
                onInput={(e) => patchSetProfile("dateOfBirth", e.target.value)}
                disabled={!isEditingInfo}
                style={{ display: "block", margin: "0.5rem 0px" }}
              ></input>
            </div>
            <div style={{ float: "left", margin: "0 1.5rem 0 0" }}>
              <b>Gender</b>
              <input
                type="text"
                value={profile.gender}
                onInput={(e) => patchSetProfile("gender", e.target.value)}
                disabled={!isEditingInfo}
                style={{ display: "block", margin: "0.5rem 0px" }}
              ></input>
            </div>
          </div>
          <div>
            <div>
              <b>Phone number</b>
              <input
                type="text"
                value={profile.phoneNumber}
                onInput={(e) => patchSetProfile("phoneNumber", e.target.value)}
                disabled={!isEditingInfo}
                style={{
                  display: "block",
                  margin: "0.5rem 0px",
                  padding: "0 0 0 0.25rem",
                }}
              ></input>
            </div>
            <br />
            {isEditingInfo || isEditingEmail ? (
              <>
                <button
                  onClick={async () => {
                    isEditingInfo
                      ? await updatePersonalInfo()
                      : await updateEmailAddress();
                  }}
                  style={{ margin: "0 1rem 0 0" }}
                >
                  Update
                </button>
                <button
                  onClick={() => {
                    exitEdit(false);
                  }}
                >
                  Cancel
                </button>
              </>
            ) : (
              <>
                <button
                  onClick={startEditInfo}
                  style={{ margin: "0 1.5rem 0 0" }}
                >
                  Edit personal info
                </button>
                <button
                  onClick={startEditEmail}
                  style={{ margin: "0 1.5rem 0 0" }}
                >
                  Change email address
                </button>
              </>
            )}
          </div>
        </>
      )}
    </>
  );
}
