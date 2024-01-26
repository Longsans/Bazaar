import { ApiEndpoints } from "./ApiEndpoints";

export default class SellerApi {
  static async getSellerById(sellerId) {
    const endpoint = ApiEndpoints.seller(sellerId);
    const response = await fetch(endpoint);
    if (response.ok) return await response.json();
    if (response.status === 404) return null;
    throw response.statusText;
  }
}
