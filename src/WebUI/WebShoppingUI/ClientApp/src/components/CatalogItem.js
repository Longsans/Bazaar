import React, { useState } from "react";
import { ApiEndpoints } from "../constants/ApiEndpoints";

export default function CatalogItem({
  productId,
  productName,
  productDesc,
  price,
  availStock,
  sellerId,
}) {
  const [buyQuantity, setBuyQuantity] = useState(1);

  async function addProductToBasket(productId, quantity) {
    var request = new Request(ApiEndpoints.basketItems("SPER-1"), {
      method: "POST",
      body: JSON.stringify({
        productId: productId,
        quantity: quantity,
      }),
      headers: new Headers({
        "Content-Type": "application/json",
      }),
    });
    var response = await fetch(request);
    if (!response.ok) {
      var respBody = await response.json();
      alert(respBody.error);
    }
  }

  function increaseQuantity() {
    if (buyQuantity < 100) setBuyQuantity(buyQuantity + 1);
  }

  function decreaseQuantity() {
    if (buyQuantity > 1) setBuyQuantity(buyQuantity - 1);
  }

  return (
    <tr key={productId}>
      <td>{productId}</td>
      <td>{productName}</td>
      <td style={{ width: 750 }}>{productDesc}</td>
      <td>${price}</td>
      <td>{availStock}</td>
      <td>{sellerId}</td>
      <div>
        <button onClick={() => addProductToBasket(productId, buyQuantity)}>
          Add to basket
        </button>
        <div style={{ display: "inline-block" }}>
          <button onClick={decreaseQuantity}>-</button>
          <input
            type="text"
            value={buyQuantity}
            readOnly
            style={{ display: "inline", flexDirection: "row", width: "40px" }}
          />
          <button onClick={increaseQuantity}>+</button>
        </div>
      </div>
    </tr>
  );
}
