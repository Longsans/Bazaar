import { useState, useEffect } from "react";

export function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getOrdersAsync = async () => {
      const response = await fetch("api/orders?sellerId=PNER-1");
      const data = await response.json();
      setOrders(data);
    };
    getOrdersAsync();
  }, []);

  useEffect(() => {
    setLoading(!orders.length);
  }, [orders]);

  return (
    <>
      <h1 id="tableLabel">Seller's orders</h1>
      <p>These are the orders made for your products.</p>
      {loading ? (
        <p>
          <em>Loading...</em>
        </p>
      ) : (
        <table className="table table-striped" aria-labelledby="tableLabel">
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Total</th>
              <th>Shipping address</th>
              <th>Buyer ID</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id}>
                <td>{order.id}</td>
                <td>{order.total}</td>
                <td>{order.shippingAddress}</td>
                <td>{order.buyerId}</td>
                <td>{order.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
