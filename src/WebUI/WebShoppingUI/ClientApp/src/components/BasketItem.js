import React, { useEffect, useState } from "react";
import "../site.scss";

export default function BasketItem({
  item,
  basketUpdating,
  setBasketUpdating,
  changeItemQuantity,
}) {
  const [isEditingInput, setIsEditingInput] = useState(false);
  const [quantity, setQuantity] = useState(item.quantity);
  const [quantityInput, setQuantityInput] = useState(item.quantity);

  const updateQuantity = async (qty) => {
    setBasketUpdating(true);
    try {
      await changeItemQuantity(item.productId, qty);
      if (qty > 0) alert("Quantity updated");
      else alert("Item removed");
    } catch (error) {
      alert(JSON.stringify(error));
    }
    setBasketUpdating(false);
  };

  const onQuantitySelectionChange = async (event) => {
    if (event.target.value == quantity) return;
    setQuantity(event.target.value);
    if (event.target.value < 10) await updateQuantity(event.target.value);
  };

  useEffect(() => {
    if (quantity >= 10) setIsEditingInput(true);
    else setIsEditingInput(false);
  }, [quantity]);

  return (
    <tr key={item.productId}>
      <td>{item.productId}</td>
      <td>{item.productName}</td>
      <td>${item.unitPrice}</td>
      <td>
        <form
          onSubmit={async (event) => {
            event.preventDefault();
            await updateQuantity(quantityInput);
            setQuantity(quantityInput);
          }}
        >
          <div className="d-flex align-items-center">
            {isEditingInput ? (
              <>
                <input
                  type="text"
                  value={quantityInput}
                  onInput={(event) => {
                    setQuantityInput(event.target.value.replace(/\D/, ""));
                  }}
                  maxLength={3}
                  className="form-control w-25 d-inline me-3"
                />
                {!!quantityInput && (
                  <button
                    type="submit"
                    disabled={basketUpdating}
                    className="btn baz-btn-primary slightly-small-text rounded-3 fw-semibold"
                  >
                    Update
                  </button>
                )}
              </>
            ) : (
              <select
                onChange={onQuantitySelectionChange}
                disabled={basketUpdating}
                value={quantity}
                className="form-select w-25"
                style={{ minWidth: "75px" }}
              >
                <option value={0}>0 (Remove)</option>
                <option value={1}>1</option>
                <option value={2}>2</option>
                <option value={3}>3</option>
                <option value={4}>4</option>
                <option value={5}>5</option>
                <option value={6}>6</option>
                <option value={7}>7</option>
                <option value={8}>8</option>
                <option value={9}>9</option>
                <option value={10}>10+</option>
              </select>
            )}
            <span
              className="baz-link mx-3 slightly-small-text"
              onClick={async () => await updateQuantity(0)}
            >
              Remove
            </span>
          </div>
        </form>
      </td>
      <td>{item.imageUrl}</td>
    </tr>
  );
}
