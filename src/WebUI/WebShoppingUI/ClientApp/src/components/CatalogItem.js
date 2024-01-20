import React from "react";
import { useBasket } from "../hooks/useBasket";

export default function CatalogItem({
  productId,
  productName,
  price,
  imageUrl,
}) {
  const { basket, addItemToBasket, changeItemQuantity } = useBasket();
  const itemInBasket = basket.items.filter((x) => x.productId === productId)[0];

  const addProductToBasket = async (productId) => {
    const basketChange = !itemInBasket
      ? async () => await addItemToBasket(productId, 1)
      : async () =>
          await changeItemQuantity(productId, itemInBasket.quantity + 1);
    try {
      await basketChange();
    } catch (error) {
      alert(error.error);
    }
  };

  const getQuantityIndicator = () => {
    if (!itemInBasket) {
      console.log(`item ${productId} not in basket.`);
      return <></>;
    }
    return (
      <div
        style={{
          display: "flex",
          flexDirection: "row",
          alignItems: "center",
        }}
      >
        <small>{`${itemInBasket.quantity} in basket - `}</small>
        <button
          onClick={async () => await changeItemQuantity(productId, 0)}
          style={{
            border: "none",
            background: "none",
          }}
        >
          <small>Remove</small>
        </button>
      </div>
    );
  };

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        margin: "16px",
      }}
    >
      <img src={imageUrl} style={{ maxWidth: "230px", height: "350px" }} />
      <h5 style={{ margin: "0", padding: "4px" }}>{productName}</h5>
      <h5 style={{ margin: "0", padding: "4px", fontWeight: "bold" }}>
        ${price}
      </h5>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
        }}
      >
        <button
          onClick={async () => await addProductToBasket(productId)}
          style={{ margin: "5px 0 0 0", width: "120px" }}
        >
          Add to basket
        </button>
        {getQuantityIndicator()}
      </div>
    </div>
  );
}
