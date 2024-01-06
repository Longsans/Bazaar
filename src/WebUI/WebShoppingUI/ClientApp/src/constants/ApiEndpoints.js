export class ApiEndpoints {
  static fetchProfile(userId) {
    return `api/shoppers/${userId}`;
  }
  static fetchCatalog = "api/catalog";
  static fetchBasket(userId) {
    return `api/basket/${userId}`;
  }
  static fetchOrders(userId) {
    return `api/orders?buyerId=${userId}`;
  }
}
