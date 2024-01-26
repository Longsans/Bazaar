export class ApiEndpoints {
  static profile(userId) {
    return `api/shoppers/${userId}`;
  }
  static catalog = "api/catalog";
  static basket(userId) {
    return `api/basket/${userId}`;
  }
  static orders(userId) {
    return `api/orders?buyerId=${userId}`;
  }

  static productsByNameSubstring(nameSubstring) {
    return `api/catalog?nameSubstring=${nameSubstring}`;
  }
  static product(productId) {
    return `api/catalog/${productId}`;
  }
  static seller(sellerId) {
    return `api/sellers/${sellerId}`;
  }

  static basket(userId) {
    return `api/baskets/${userId}`;
  }
  static allBasketItems(userId) {
    return `api/baskets/${userId}/items`;
  }
  static basketItem(userId, productId) {
    return `api/baskets/${userId}/items/${productId}`;
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
