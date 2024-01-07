import { useState, useEffect } from "react";
import { ApiEndpoints } from "../constants/ApiEndpoints";

export function Profile() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getProfileAsync = async () => {
      const response = await fetch(ApiEndpoints.fetchProfile("SPER-1"));
      const data = await response.json();
      setProfile(data);
    };
    getProfileAsync();
  }, []);

  useEffect(() => {
    setLoading(!profile);
  }, [profile]);

  return (
    <>
      <h1>Profile</h1>
      <br />
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <table className="table table-striped">
          <thead>
            <th>ID</th>
            <th>First name</th>
            <th>Last name</th>
            <th>Email</th>
            <th>Phone number</th>
            <th>Date of birth</th>
            <th>Gender</th>
          </thead>
          <tbody>
            <tr key={profile.id}>
              <td>{profile.id}</td>
              <td>{profile.firstName}</td>
              <td>{profile.lastName}</td>
              <td>{profile.emailAddress}</td>
              <td>{profile.phoneNumber}</td>
              <td>{profile.dateOfBirth}</td>
              <td>{profile.gender}</td>
            </tr>
          </tbody>
        </table>
      )}
    </>
  );
}
