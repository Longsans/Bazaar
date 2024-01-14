import { ApiEndpoints } from "./ApiEndpoints";
import http from "../utils/Http";

export default class BasketApi {
  static async getBasket(userId) {
    const endpoint = ApiEndpoints.basket(userId);
    return await fetch(endpoint);
  }

  static async addItemToBasket(userId, productId, quantity) {
    var response = await http.postAsync(ApiEndpoints.basketItems(userId), {
      productId: productId,
      quantity: quantity,
    });
    if (response.ok) {
      return await response.json();
    } else {
      throw await response.json();
    }
  }

  static async checkout(checkoutInfo) {
    var response = await http.postAsync(ApiEndpoints.checkouts, checkoutInfo);
    if (!response.ok) throw await response.json();
  }
}
