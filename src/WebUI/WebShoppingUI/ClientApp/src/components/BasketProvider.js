import React from "react";
import { useProvideBasket } from "../hooks/useBasket";
import { BasketContext } from "../hooks/useBasket";

export default function BasketProvider({ children }) {
  const basketContext = useProvideBasket();

  return (
    <BasketContext.Provider value={basketContext}>
      {children}
    </BasketContext.Provider>
  );
}
