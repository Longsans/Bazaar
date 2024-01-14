import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
import { Link } from "react-router-dom";

export default function Basket() {
  const [basket, setBasket] = useState(null);
  const waltuh = "SPER-1";

  const getBasket = async () => {
    const endpoint = ApiEndpoints.basket(waltuh);
    const response = await fetch(endpoint);
    setBasket(await response.json());
  };

  useEffect(() => {
    getBasket();
  }, []);

  return (
    <>
      <h1>Basket</h1>
      <br />
      <table className="table table-striped">
        <thead>
          <th>Product ID</th>
          <th>Product name</th>
          <th>Unit price</th>
          <th>Quantity</th>
          <th>Image URL</th>
        </thead>
        <tbody>
          {basket && basket.items.length ? (
            basket.items.map((x) => (
              <tr key={x.productId}>
                <td>{x.productId}</td>
                <td>{x.productName}</td>
                <td>${x.unitPrice}</td>
                <td>{x.quantity}</td>
                <td>{x.imageUrl}</td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={5}>No items in basket.</td>
            </tr>
          )}
        </tbody>
      </table>
      <h5 style={{ fontWeight: "bold" }}>
        Total: ${basket ? basket.total : 0}
      </h5>
      <Link to="/checkout">
        <button disabled={!basket?.items.length}>Proceed to checkout</button>
      </Link>
    </>
  );
}
