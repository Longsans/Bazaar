export default class HttpUtility {
  static async getAsync(url, fnSuccess, fnError) {
    var response = await fetch(url);
    if (response.ok) {
      fnSuccess(response);
    } else {
      fnError(response);
    }
  }

  static async postAsync(url, body, fnSuccess, fnError) {
    await this.makeRequestWithBody(url, "POST", body, fnSuccess, fnError);
  }

  static async putAsync(url, body, fnSuccess, fnError) {
    await this.makeRequestWithBody(url, "PUT", body, fnSuccess, fnError);
  }

  static async patchAsync(url, body, fnSuccess, fnError) {
    await this.makeRequestWithBody(url, "PATCH", body, fnSuccess, fnError);
  }

  static async makeRequestWithBody(url, method, body, fnSuccess, fnError) {
    var request = new Request(url, {
      method: method,
      body: JSON.stringify(body),
      headers: new Headers({
        "Content-Type": "application/json",
      }),
    });
    var response = await fetch(request);
    if (response.ok) {
      fnSuccess(response);
    } else {
      fnError(response);
    }
  }
}
