import { ApiEndpoints } from "./ApiEndpoints";

export default class CatalogApi {
  static async getProductsWhoseNameContain(nameSubstring) {
    const endpoint = ApiEndpoints.productsByNameSubstring(nameSubstring);
    const response = await fetch(endpoint);
    return await response.json();
  }

  static async getProductByProductId(productId) {
    const endpoint = ApiEndpoints.product(productId);
    const response = await fetch(endpoint);
    if (response.ok) return await response.json();
    if (response.status == 404) return null;
    throw response.statusText;
  }
}
