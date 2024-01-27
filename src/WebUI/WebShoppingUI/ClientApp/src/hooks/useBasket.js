import { useState, createContext, useContext, useEffect } from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
import http from "../utils/Http";

const BasketContext = createContext(null);

const useBasket = () => {
  return useContext(BasketContext);
};

const useProvideBasket = () => {
  const [basket, setBasket] = useState(null);
  // todo: use an auth context to get user ID instead of this
  const waltuh = "SPER-1";

  const pullBasket = async () => {
    const response = await fetch(ApiEndpoints.basket(waltuh));
    setBasket(await response.json());
  };

  const addItemToBasket = async (productId, quantity) => {
    var response = await http.postAsync(ApiEndpoints.allBasketItems(waltuh), {
      productId: productId,
      quantity: quantity,
    });
    if (response.ok) await pullBasket();
    else throw await response.json();
  };

  const changeItemQuantity = async (productId, quantity) => {
    var response = await http.patchAsync(
      ApiEndpoints.basketItem(waltuh, productId),
      quantity
    );
    if (response.ok) await pullBasket();
    else throw await response.json();
  };

  const checkout = async (checkoutInfo) => {
    var response = await http.postAsync(ApiEndpoints.checkouts, checkoutInfo);
    if (response.ok) await pullBasket();
    else throw await response.json();
  };

  useEffect(() => {
    pullBasket();
  }, []);

  return { basket, addItemToBasket, changeItemQuantity, checkout };
};

export { BasketContext, useBasket, useProvideBasket };
