import { useState, useEffect } from "react";

export function Catalog() {
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getCatalogAsync = async () => {
      const response = await fetch("api/catalog?sellerId=PNER-1");
      const data = await response.json();
      setCatalog(data);
    };
    getCatalogAsync();
  }, []);

  useEffect(() => {
    setLoading(!catalog.length);
  }, [catalog]);

  return (
    <div>
      <h1 id="tableLabel">Product catalog</h1>
      <p>These are the products your have posted on your shop's catalog.</p>
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <table className="table table-striped" aria-labelledby="tableLabel">
          <thead>
            <tr>
              <th>Product ID</th>
              <th>Name</th>
              <th>Description</th>
              <th>Price</th>
              <th>Available stock</th>
              <th>Restock threshold</th>
              <th>Max stock threshold</th>
              <th>Seller ID</th>
            </tr>
          </thead>
          <tbody>
            {catalog.map((item) => (
              <tr key={item.productId}>
                <td>{item.productId}</td>
                <td>{item.name}</td>
                <td>{item.description}</td>
                <td>{item.price}</td>
                <td>{item.availableStock}</td>
                <td>{item.restockThreshold}</td>
                <td>{item.maxStockThreshold}</td>
                <td>{item.sellerId}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
