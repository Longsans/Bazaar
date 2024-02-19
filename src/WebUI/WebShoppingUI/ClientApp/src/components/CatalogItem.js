import React from "react";
import { useBasket } from "../hooks/useBasket";
import "../site.scss";

export default function CatalogItem({
  productId,
  productName,
  price,
  imageUrl,
  onItemClick,
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
      return <></>;
    }
    return (
      <div className="d-flex flex-row align-items-center mt-1">
        <span className="small-text baz-text-secondary fw-bold">{`${itemInBasket.quantity} in basket - `}</span>

        <span
          onClick={async (event) => {
            event.stopPropagation();
            await changeItemQuantity(productId, 0);
          }}
          className="btn btn-link d-inline p-0 ms-1 baz-link small-text"
        >
          Remove
        </span>
      </div>
    );
  };

  return (
    <div className="d-flex flex-column align-items-start m-2 p-2 border border-1 border-light-subtle pb-3 rounded-1">
      <div
        className="bg-secondary bg-opacity-10 d-flex flex-row align-items-center rounded-1"
        style={{ width: "250px", height: "250px", cursor: "pointer" }}
        onClick={onItemClick}
      >
        <img
          src={imageUrl}
          style={{ maxWidth: "80%", maxHeight: "80%" }}
          className="d-block m-auto"
        />
      </div>
      <h5 className="m-1 baz-a" onClick={onItemClick}>
        {productName}
      </h5>
      <h5 className="mx-1 fw-bold">${price}</h5>
      <button
        onClick={async (event) => {
          event.stopPropagation();
          await addProductToBasket(productId);
        }}
        className="btn baz-btn-primary mt-2 rounded-3 semi-bold small-text"
      >
        Add to basket
      </button>
      {getQuantityIndicator()}
    </div>
  );
}
