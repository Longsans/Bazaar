import React, { useState } from "react";
import BasketApi from "../api/BasketApi";

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
    try {
      await BasketApi.addItemToBasket("SPER-1", productId, quantity);
      alert("Product added to basket.");
    } catch (error) {
      alert(error.error);
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
