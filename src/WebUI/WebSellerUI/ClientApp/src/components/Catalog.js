import { useState, useEffect } from 'react';

export function Catalog(props) {
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getCatalogAsync = async () => {
      const response = await fetch('api/catalog');
      const data = await response.json();
      setCatalog(data);
    };
    getCatalogAsync();
  }, []);

  useEffect(() => {
    if (catalog.length)
      setLoading(false);
    else setLoading(true);
  }, [catalog]);

  return (
    <div>
      <h1 id="tableLabel">Product catalog</h1>
      <p>This component demonstrates fetching data from a remote API.</p>
      {
        loading ?
          <p><em>Loading...</em></p> :
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
              {catalog.map(item =>
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
              )}
            </tbody>
          </table>
      }
    </div>
  );
}
