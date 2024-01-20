import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
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
      <h2 style={{ fontWeight: "bold" }}>Product catalog</h2>
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
      <h4 style={{ fontWeight: "bold" }}>Results</h4>
      {loading ? (
        <em>Loading catalog...</em>
      ) : !catalog.length ? (
        <h5>No result.</h5>
      ) : (
        <div style={{ display: "flex", flexDirection: "row" }}>
          {catalog.map((x) => (
            <CatalogItem
              productId={x.productId}
              productName={x.productName}
              productDesc={x.productDescription}
              imageUrl={x.imageUrl}
              price={x.price}
              availStock={x.availableStock}
              sellerId={x.sellerId}
            />
          ))}
        </div>
      )}
    </>
  );
}
