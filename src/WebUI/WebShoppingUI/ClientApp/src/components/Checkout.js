import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useBasket } from "../hooks/useBasket";
import { useNavigate } from "react-router-dom";
import "../site.scss";
import {
  RegisteredTextField,
  RegisteredNumberField,
  RegisteredDateField,
} from "../components/FormElements";

export default function Checkout() {
  const { register, handleSubmit } = useForm();
  const { basket, checkout } = useBasket();
  const navigate = useNavigate();
  const textFields = {
    city: "City",
    country: "Country",
    zipCode: "Zip code",
    shippingAddress: "Shipping address",
  };

  const onSubmit = async (data) => {
    try {
      const checkoutInfo = {
        buyerId: "SPER-1",
        ...data,
      };
      await checkout({
        ...checkoutInfo,
        cardNumber: "" + checkoutInfo.cardNumber,
        cardSecurityNumber: "" + checkoutInfo.cardSecurityNumber,
      });
      alert(
        "Your order is being processed. You can follow its status on Order history page."
      );
    } catch (error) {
      alert(JSON.stringify(error));
    }
  };

  useEffect(() => {
    if (basket && !basket.items.length) navigate("/catalog");
  }, [basket]);

  return (
    <>
      <h1>Checkout</h1>
      <br />
      <form onSubmit={handleSubmit(onSubmit)}>
        <h5 className="semi-bold">Shipping information</h5>
        <hr />
        <div className="d-flex mb-5">
          {Object.entries(textFields).map(([name, labelText]) => (
            <RegisteredTextField {...{ name, labelText, register }} />
          ))}
        </div>
        <h5 className="semi-bold">Payment information</h5>
        <hr />
        <div className="d-flex mb-5">
          <RegisteredNumberField
            name="cardNumber"
            labelText="Card number"
            register={register}
          />
          <RegisteredTextField
            name="cardHolderName"
            labelText="Card holder name"
            register={register}
          />

          <RegisteredNumberField
            name="cardSecNumber"
            labelText="Card security number"
            register={register}
          />
          <RegisteredDateField
            name="cardExp"
            labelText="Card expiration date"
            register={register}
          />
        </div>
        <div>
          <button type="submit" className="btn baz-btn-primary">
            Checkout
          </button>
          <span
            className="btn btn-secondary ms-3"
            onClick={() => navigate("/basket")}
          >
            Back to basket
          </span>
        </div>
      </form>
    </>
  );
}
