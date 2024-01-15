import React, { useEffect, useState } from "react";
import { ApiEndpoints } from "../api/ApiEndpoints";
import Order from "./Order";
import { OrderStatus } from "../constants/OrderStatus.ts";

export default function OrderHistory() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const inProgressSelection = "inProgress";
  const completedSelection = "completed";
  const cancelledSelection = "cancelled";
  const inProgressFilter = [
    OrderStatus.PendingValidation,
    OrderStatus.ProcessingPayment,
    OrderStatus.PendingSellerConfirmation,
    OrderStatus.Shipping,
    OrderStatus.Postponed,
  ];
  const completedFilter = [OrderStatus.Shipped];
  const cancelledFilter = [OrderStatus.Cancelled];
  const [statusFilter, setStatusFilter] = useState(inProgressFilter);
  const [filteredOrders, setFilteredOrders] = useState([]);

  const fetchOrders = async () => {
    setLoading(true);
    var response = await fetch(ApiEndpoints.orders("SPER-1"));
    if (response.ok) {
      setOrders(await response.json());
    }
    setLoading(false);
  };

  const onSelectionChange = (event) => {
    let newFilter;
    switch (event.target.value) {
      case inProgressSelection:
        newFilter = inProgressFilter;
        break;
      case completedSelection:
        newFilter = completedFilter;
        break;
      case cancelledSelection:
        newFilter = cancelledFilter;
        break;
    }
    setStatusFilter(newFilter);
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  useEffect(() => {
    setFilteredOrders(orders.filter((x) => statusFilter.includes(x.status)));
  }, [orders, statusFilter]);

  return (
    <>
      <h1>Order history</h1>
      <br />
      <div
        style={{
          display: "flex",
          alignItems: "center",
          margin: "0 0 0.5rem 0",
        }}
      >
        <h5 style={{ margin: "0" }}>Filter by</h5>
        <select style={{ margin: "0 0.5rem" }} onChange={onSelectionChange}>
          <option value={inProgressSelection}>Orders in progress</option>
          <option value={completedSelection}>Orders completed</option>
          <option value={cancelledSelection}>Orders cancelled</option>
        </select>
      </div>
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
          ) : filteredOrders.length ? (
            filteredOrders.map((x) => (
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
          ) : (
            <tr>
              <td colSpan={6}>No result</td>
            </tr>
          )}
        </tbody>
      </table>
    </>
  );
}
