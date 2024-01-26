import React from "react";
import { useForm } from "react-hook-form";
import { useBasket } from "../hooks/useBasket";
import { useNavigate } from "react-router-dom";
import "../site.scss";

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

  const registeredTextField = (name, labelText, register) => (
    <div className="me-4">
      <span className="form-label semi-bold">{labelText}</span>
      <input
        type={"text"}
        {...register(name, { required: true })}
        className="form-control mt-2"
      />
    </div>
  );

  const registeredNumberField = (name, labelText, register) => (
    <div className="me-4">
      <span className="form-label semi-bold">{labelText}</span>
      <input
        type={"number"}
        {...register(name, {
          required: true,
          valueAsNumber: true,
          validate: (value) => value > 0,
        })}
        className="form-control mt-2"
      />
    </div>
  );

  const registeredDateField = (name, labelText, register) => (
    <div className="me-4">
      <span className="form-label semi-bold">{labelText}</span>
      <input
        type="date"
        {...register(name, {
          required: true,
          valueAsDate: true,
        })}
        className="form-control mt-2"
      />
    </div>
  );

  return (
    <>
      <h1>Checkout</h1>
      <br />
      <form onSubmit={handleSubmit(onSubmit)}>
        <h5 className="semi-bold">Shipping information</h5>
        <hr />
        <div style={{ display: "flex", margin: "0 0 2.5rem 0" }}>
          {Object.entries(textFields).map(([name, labelText]) =>
            registeredTextField(name, labelText, register)
          )}
        </div>
        <h5 className="semi-bold">Payment information</h5>
        <hr />
        <div style={{ display: "flex" }}>
          {registeredNumberField("cardNumber", "Card number", register)}
          {registeredTextField("cardHolderName", "Card holder name", register)}
          {registeredNumberField(
            "cardSecurityNumber",
            "Card security number",
            register
          )}
          {registeredDateField(
            "cardExpirationDate",
            "Card expiration date",
            register
          )}
        </div>
        <br />
        <button type="submit" className="btn baz-btn-primary">
          Checkout
        </button>
        <span
          className="btn btn-secondary ms-3"
          onClick={() => navigate("/basket")}
        >
          Back to basket
        </span>
      </form>
    </>
  );
}
