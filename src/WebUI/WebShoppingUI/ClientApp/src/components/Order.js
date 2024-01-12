import React from "react";
import { ApiEndpoints } from "../backend/ApiEndpoints";

export default function Order({
  orderId,
  items,
  total,
  shippingAddress,
  status,
  cancelReason,
  fnRefreshOrders,
}) {
  const cancelOrder = async () => {
    var url = ApiEndpoints.orderById(orderId);
    var request = new Request(url, {
      method: "PATCH",
      body: JSON.stringify({
        reason: "waltuh, i'm not doing it waltuh",
      }),
      headers: new Headers({
        "Content-Type": "application/json",
        "X-CSRF": "1",
      }),
    });
    var response = await fetch(request);
    if (response.ok) {
      fnRefreshOrders();
    } else {
      var respBody = await response.json();
      alert(respBody.error);
    }
  };

  return (
    <>
      <tr key={orderId}>
        <td>{orderId}</td>
        <td>
          <div>
            {items.map((i) => (
              <p>{`${i.quantity} x ${i.productName} (${i.productId})`}</p>
            ))}
          </div>
        </td>
        <td>${total}</td>
        <td>{shippingAddress}</td>
        <td>{status}</td>
        <td>{cancelReason ? cancelReason : "-"}</td>
        {status !== "Cancelled" && (
          <button onClick={cancelOrder}>Cancel</button>
        )}
      </tr>
    </>
  );
}
