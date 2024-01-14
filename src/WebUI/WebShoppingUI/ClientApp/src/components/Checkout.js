import React from "react";
import { RegisteredInput, RegisteredNumberField } from "./FormElements";
import { useForm } from "react-hook-form";
import BasketApi from "../api/BasketApi";

export default function Checkout() {
  const { register, handleSubmit } = useForm();
  const textFields = {
    city: "City",
    country: "Country",
    zipCode: "Zip code",
    shippingAddress: "Shipping address",
  };

  const onSubmit = async (data) => {
    try {
      await BasketApi.checkout({
        buyerId: "SPER-1",
        ...data,
      });
      alert(
        "Your order is being processed. You can follow its status on Order history page."
      );
    } catch (error) {
      alert(error);
    }
  };

  return (
    <>
      <h1>Checkout</h1>
      <br />
      <form onSubmit={handleSubmit(onSubmit)}>
        <h5>Shipping information</h5>
        <hr />
        <div style={{ display: "flex", margin: "0 0 2.5rem 0" }}>
          {Object.entries(textFields).map(([name, labelText]) => (
            <RegisteredInput
              name={name}
              labelText={labelText}
              type="text"
              register={register}
              disabled={false}
            />
          ))}
        </div>
        <h5>Payment information</h5>
        <hr />
        <div style={{ display: "flex" }}>
          <RegisteredNumberField
            name="cardNumber"
            labelText="Card number"
            register={register}
            disabled={false}
          />
          <RegisteredInput
            name="cardHolderName"
            labelText="Card holder name"
            type="text"
            register={register}
            disabled={false}
          />
          <RegisteredNumberField
            name="cardSecurityNumber"
            labelText="Card security number"
            register={register}
            disabled={false}
          />
          <RegisteredInput
            name="cardExpiration"
            labelText="Card expiration date"
            type="date"
            register={register}
            disabled={false}
          />
        </div>
        <br />
        <button type="submit">Checkout</button>
      </form>
    </>
  );
}
