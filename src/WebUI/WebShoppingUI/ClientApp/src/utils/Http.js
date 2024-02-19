export default class HttpUtility {
  static async postAsync(url, body) {
    return await this.makeRequestWithBody(url, "POST", body);
  }

  static async putAsync(url, body) {
    return await this.makeRequestWithBody(url, "PUT", body);
  }

  static async patchAsync(url, body) {
    return await this.makeRequestWithBody(url, "PATCH", body);
  }

  static async makeRequestWithBody(url, method, body) {
    var request = new Request(url, {
      method: method,
      body: JSON.stringify(body),
      headers: new Headers({
        "Content-Type": "application/json",
      }),
    });
    return await fetch(request);
  }
}
