import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
import Order from "./Order";

export default function OrderHistory() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);

  const fetchOrders = async () => {
    setLoading(true);
    var response = await fetch(ApiEndpoints.orders("SPER-1"));
    if (response.ok) {
      setOrders(await response.json());
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  return (
    <>
      <h1>Order history</h1>
      <br />
      <table className="table table-striped">
        <thead>
          <th>Order ID</th>
          <th>Items</th>
          <th>Total</th>
          <th>Shipping address</th>
          <th>Status</th>
          <th>Cancel reason</th>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan={6}>Loading...</td>
            </tr>
          ) : !orders.length ? (
            <tr>
              <td colSpan={6}>No orders placed.</td>
            </tr>
          ) : (
            orders.map((x) => (
              <Order
                orderId={x.id}
                items={x.items}
                total={x.total}
                shippingAddress={x.shippingAddress}
                status={x.status}
                cancelReason={x.cancelReason}
                fnRefreshOrders={fetchOrders}
              />
            ))
          )}
        </tbody>
      </table>
    </>
  );
}
