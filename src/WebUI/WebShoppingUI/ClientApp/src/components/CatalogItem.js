import React, { useState } from "react";
import { useBasket } from "../hooks/useBasket";

export default function CatalogItem({
  productId,
  productName,
  productDesc,
  price,
  imageUrl,
  availStock,
  sellerId,
}) {
  const [buyQuantity, setBuyQuantity] = useState(1);
  const { addItemToBasket } = useBasket();

  async function addProductToBasket(productId, quantity) {
    try {
      await addItemToBasket(productId, quantity);
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
      <td>
        <img src={imageUrl} />
      </td>
      <td>{productName}</td>
      <td>${price}</td>
      <div>
        <button
          onClick={async () => await addProductToBasket(productId, buyQuantity)}
        >
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
