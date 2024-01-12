import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../backend/ApiEndpoints";

export default function Basket() {
  const [basket, setBasket] = useState(null);
  const waltuh = "SPER-1";

  const getBasket = async () => {
    const endpoint = ApiEndpoints.basket(waltuh);
    const response = await fetch(endpoint);
    setBasket(await response.json());
  };

  const checkout = async () => {
    var request = new Request(ApiEndpoints.checkouts, {
      method: "POST",
      body: JSON.stringify({
        BuyerId: waltuh,
        City: "Albuquerque, New Mexico",
        Country: "U.S.A",
        ZipCode: "73000",
        ShippingAddress: "308 Negra Arroyo Lane",
        CardNumber: "123456",
        CardHolderName: "Walter White",
        CardExpiration: "2026-11-12",
        CardSecurityNumber: "456789",
      }),
      headers: new Headers({
        "Content-Type": "application/json",
      }),
    });
    var response = await fetch(request);
    if (response.ok) {
      getBasket();
      alert(
        "Checkout accepted, newly placed order can be seen in your orders list."
      );
    } else {
      var respBody = await response.json();
      alert(respBody.error);
    }
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
      <button disabled={!basket?.items.length} onClick={checkout}>
        Checkout
      </button>
    </>
  );
}
