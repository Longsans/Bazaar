import { useState, useEffect } from "react";

export function Contracts() {
  const [contracts, setContracts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getContractsAsync = async () => {
      const response = await fetch("api/contracts?partnerId=PNER-1");
      const data = await response.json();
      setContracts(data);
    };
    getContractsAsync();
  }, []);

  useEffect(() => {
    setLoading(!contracts.length);
  }, [contracts]);

  return (
    <>
      <h1 id="tableLabel">Seller's Contracts</h1>
      <p>These are the contracts you have signed with Bazaar.</p>
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <table className="table table-striped" aria-labelledby="tableLabel">
          <thead>
            <tr>
              <th>Contract ID</th>
              <th>Partner ID</th>
              <th>Selling plan ID</th>
              <th>Start date</th>
              <th>End date</th>
            </tr>
          </thead>
          <tbody>
            {contracts.map((contract) => (
              <tr key={contract.id}>
                <td>{contract.id}</td>
                <td>{contract.partnerId}</td>
                <td>{contract.sellingPlanId}</td>
                <td>
                  {new Date(contract.startDate).toLocaleDateString("en-US")}
                </td>
                <td>
                  {contract.endDate != null
                    ? new Date(contract.endDate).toLocaleString("en-US")
                    : "Indefinite"}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
