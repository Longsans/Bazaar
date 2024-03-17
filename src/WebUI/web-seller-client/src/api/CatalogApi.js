import axios from "axios";

export default class CatalogApi {
  static httpClient = axios.create({
    baseURL: `${process.env.REACT_APP_GW_URL}/pc/api/`,
    timeout: 60000,
  });

  static async sendGetRequest(url) {
    return (await this.httpClient.get(url)).data;
  }
  static async sendPostRequest(url, data) {
    return (await this.httpClient.post(url, data)).data;
  }
  static async sendPatchRequest(url, data) {
    return (await this.httpClient.patch(url, data)).data;
  }

  static async getCatalogByCriteria({
    sellerId,
    categoryId,
    nameSubstring,
    includeDeleted,
  }) {
    return await this.sendGetRequest(
      this.endpointUris.catalogByCriteria({
        sellerId,
        categoryId,
        nameSubstring,
        includeDeleted,
      })
    );
  }

  static async createListing(productListing) {
    return await this.sendPostRequest(
      this.endpointUris.catalog,
      productListing
    );
  }

  static async updateListingInfo(productId, listingInfo) {
    return await this.sendPatchRequest(
      this.endpointUris.listing(productId),
      listingInfo
    );
  }

  static async changeMerchantStock(productId, stockChange) {
    return await this.sendPatchRequest(
      this.endpointUris.listingStock(productId),
      stockChange
    );
  }

  static async changeListingStatus(productId, listingStatus) {
    await this.sendPatchRequest(this.endpointUris.listingStatus(productId), {
      status: listingStatus,
    });
  }

  static async changeSubcategory(productId, categoryId) {
    return await this.sendPatchRequest(
      this.endpointUris.listingSubcategory(productId),
      {
        subcategoryId: categoryId,
      }
    );
  }

  static async getProductCategories(nameSubstring) {
    return await this.sendGetRequest(
      this.endpointUris.productCategories(nameSubstring)
    );
  }

  // endpoint addresses
  static endpointUris = {
    catalog: `catalog`,
    catalogByCriteria(criteria = {}) {
      let endpoint = this.catalog;
      let queryStarted = false;
      for (const c in criteria) {
        if (!criteria[c]) continue;
        endpoint += queryStarted ? "&" : "?";
        endpoint += `${c}=${criteria[c]}`;
        queryStarted = true;
      }

      return endpoint;
    },
    listing(productId) {
      return this.catalog + `/${productId}`;
    },
    listingStock(productId) {
      return this.catalog + `/${productId}/stock`;
    },
    listingStatus(productId) {
      return this.catalog + `/${productId}/listing-status`;
    },
    listingSubcategory(productId) {
      return this.catalog + `/${productId}/category`;
    },
    productCategories(nameSubstring) {
      return `product-categories?nameSubstring=${nameSubstring}`;
    },
  };
}
