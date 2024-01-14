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
    return `api/catalog?nameSubstring=${nameSubstring}`;
  }

  static basket(userId) {
    return `api/baskets/${userId}`;
  }
  static basketItems(userId) {
    return `api/baskets/${userId}/items`;
  }
  static checkouts = "api/checkouts";

  static orders(userId) {
    return `api/orders?buyerId=${userId}`;
  }
  static orderById(orderId) {
    return `api/orders/${orderId}`;
  }

  static userPersonalInfo = (userId) => `api/shoppers/${userId}/personal-info`;
  static userEmailAddress = (userId) => `api/shoppers/${userId}/email-address`;
}
