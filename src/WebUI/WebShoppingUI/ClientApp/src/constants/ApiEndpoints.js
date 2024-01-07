export class ApiEndpoints {
  static fetchProfile(userId) {
    return `api/shoppers/${userId}`;
  }
  static fetchCatalog = "api/catalog";
  static basket(userId) {
    return `api/basket/${userId}`;
  }
  static fetchOrders(userId) {
    return `api/orders?buyerId=${userId}`;
  }
  static fetchCatalogByNameSubstring(nameSubstring) {
    return `http://localhost:5002/api/catalog?nameSubstring=${nameSubstring}`;
  }

  static basket(userId) {
    return `http://localhost:5002/api/baskets/${userId}`;
  }
  static basketItems(userId) {
    return `http://localhost:5002/api/baskets/${userId}/items`;
  }
  static checkouts = "http://localhost:5002/api/checkouts";

  static orders(userId) {
    return `http://localhost:5002/api/orders?buyerId=${userId}`;
  }

  static orderById(orderId) {
    return `http://localhost:5002/api/orders/${orderId}`;
  }
}
