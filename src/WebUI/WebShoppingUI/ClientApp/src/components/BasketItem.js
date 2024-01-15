import React from "react";
import { RegisteredNumberField } from "./FormElements";
import { useForm } from "react-hook-form";

export default function BasketItem({
  item,
  basketUpdating,
  setBasketUpdating,
  changeItemQuantity,
}) {
  const { register, handleSubmit, reset } = useForm({ defaultValues: item });

  const onSubmit = async (data) => {
    setBasketUpdating(true);
    try {
      await changeItemQuantity(item.productId, data.quantity);
      alert("Quantity updated");
    } catch (error) {
      alert(error);
      reset(item);
    }
    setBasketUpdating(false);
  };

  return (
    <tr key={item.productId}>
      <td>{item.productId}</td>
      <td>{item.productName}</td>
      <td>${item.unitPrice}</td>
      <td>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div>
            <RegisteredNumberField
              name="quantity"
              register={register}
              disabled={false}
            />
            <button type="submit" disabled={basketUpdating}>
              Update
            </button>
          </div>
        </form>
      </td>
      <td>{item.imageUrl}</td>
    </tr>
  );
}
