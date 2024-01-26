import React from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
import { OrderStatusFormatting } from "../constants/OrderStatus.ts";
import { OrderStatus } from "../constants/OrderStatus.ts";

export default function Order({
  orderId,
  items,
  total,
  shippingAddress,
  status,
  cancelReason,
  fnRefreshOrders,
}) {
  const cancellableStatuses = [
    OrderStatus.PendingSellerConfirmation,
    OrderStatus.Postponed,
  ];

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
        <td>{OrderStatusFormatting[status]}</td>
        <td>{cancelReason ? cancelReason : "-"}</td>
        {cancellableStatuses.includes(status) && (
          <button onClick={cancelOrder}>Cancel</button>
        )}
      </tr>
    </>
  );
}
