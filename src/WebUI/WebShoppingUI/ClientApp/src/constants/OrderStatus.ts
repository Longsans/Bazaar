enum OrderStatus {
  PendingValidation = "PendingValidation",
  ProcessingPayment = "ProcessingPayment",
  PendingSellerConfirmation = "PendingSellerConfirmation",
  Shipping = "Shipping",
  Shipped = "Shipped",
  Cancelled = "Cancelled",
  Postponed = "Postponed",
}

const OrderStatusFormatting = {
  [OrderStatus.PendingValidation]: "Pending validation",
  [OrderStatus.ProcessingPayment]: "Processing payment",
  [OrderStatus.PendingSellerConfirmation]: "Pending seller confirmation",
  [OrderStatus.Shipping]: "Shipping",
  [OrderStatus.Shipped]: "Shipped",
  [OrderStatus.Cancelled]: "Cancelled",
  [OrderStatus.Postponed]: "Postponed",
};

export { OrderStatus, OrderStatusFormatting };
