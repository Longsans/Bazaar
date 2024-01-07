import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../constants/ApiEndpoints";
import CatalogItem from "./CatalogItem";

export default function Catalog() {
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState(null);

  async function fetchCatalog(event) {
    event.preventDefault();
    setLoading(true);
    const endpoint = ApiEndpoints.fetchCatalogByNameSubstring(searchTerm);
    const response = await fetch(endpoint);
    const catalogData = await response.json();
    setCatalog(catalogData);
  }

  useEffect(() => {
    setLoading(false);
  }, [catalog]);

  return (
    <>
      <h1>Product catalog</h1>
      <br />
      <form onSubmit={fetchCatalog}>
        <input
          type="text"
          value={searchTerm}
          onInput={(e) => setSearchTerm(e.target.value)}
          placeholder="Enter a phrase to search in catalog"
          style={{ width: 300 }}
        />
        <button type="submit" disabled={!searchTerm}>
          Search
        </button>
      </form>
      <br />
      <table className="table table-striped">
        <thead>
          <th>Product ID</th>
          <th>Product name</th>
          <th>Product description</th>
          <th>Price</th>
          <th>Available stock</th>
          <th>Seller ID</th>
        </thead>
        <tbody>
          {loading ? (
            <em>Loading catalog...</em>
          ) : !catalog.length ? (
            <tr>
              <td colSpan={6}>No result.</td>
            </tr>
          ) : (
            catalog.map((x) => (
              <CatalogItem
                productId={x.productId}
                productName={x.productName}
                productDesc={x.productDescription}
                price={x.price}
                availStock={x.availableStock}
                sellerId={x.sellerId}
              />
            ))
          )}
        </tbody>
      </table>
    </>
  );
}
