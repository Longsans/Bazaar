import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useBasket } from "../hooks/useBasket";
import BasketItem from "./BasketItem";

export default function Basket() {
  const { basket, changeItemQuantity } = useBasket();
  const [updating, setUpdating] = useState(false);

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
              <BasketItem
                key={x.productId}
                item={x}
                basketUpdating={updating}
                setBasketUpdating={setUpdating}
                changeItemQuantity={changeItemQuantity}
              />
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
