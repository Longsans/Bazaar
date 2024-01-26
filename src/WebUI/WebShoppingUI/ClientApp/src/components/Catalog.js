import React, { useEffect, useState } from "react";
import CatalogItem from "./CatalogItem";
import { useNavigate } from "react-router-dom";
import CatalogApi from "../api/CatalogApi";
import "../site.scss";

export default function Catalog() {
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState(null);
  const [searchedFirstTime, setSearchedFirstTime] = useState(false);
  const navigate = useNavigate();

  async function fetchCatalog(event) {
    event.preventDefault();
    if (!searchTerm) return;

    setLoading(true);
    const products = await CatalogApi.getProductsWhoseNameContain(searchTerm);
    setCatalog(products);
    setSearchedFirstTime(true);
  }

  useEffect(() => {
    setLoading(false);
  }, [catalog]);

  return (
    <>
      <h2 style={{ fontWeight: "bold" }}>Product catalog</h2>
      <br />
      <form
        onSubmit={fetchCatalog}
        className="d-flex flex-row align-items-center"
      >
        <input
          type="text"
          className="form-control d-inline border-radius-nor search-input"
          value={searchTerm}
          onInput={(e) => setSearchTerm(e.target.value)}
          placeholder="Search by part of name"
          style={{ width: 300 }}
        />
        <button className="btn baz-btn-search border-radius-nol" type="submit">
          <img
            src="/images/search.png"
            className="m-auto"
            style={{ maxHeight: "22px", maxWidth: "22px" }}
          />
        </button>
      </form>
      <br />
      <h4 style={{ fontWeight: "bold" }}>Results</h4>
      {!searchedFirstTime && (
        <h5>Enter a search term to search catalog items.</h5>
      )}
      {searchedFirstTime &&
        (loading ? (
          <em>Loading catalog...</em>
        ) : !catalog.length ? (
          <h5>No result.</h5>
        ) : (
          <div style={{ display: "flex", flexDirection: "row" }}>
            {catalog.map((x) => (
              <CatalogItem
                productId={x.productId}
                productName={x.productName}
                imageUrl={x.imageUrl}
                price={x.price}
                onItemClick={() => navigate(`/pd/${x.productId}`)}
              />
            ))}
          </div>
        ))}
    </>
  );
}
