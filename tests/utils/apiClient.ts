import { APIRequestContext, expect } from "@playwright/test";

export class ApiClient {
  private readonly request: APIRequestContext;
  private token?: string;

  constructor(request: APIRequestContext, token?: string) {
    this.request = request;
    this.token = token;
  }

  private getHeaders(): Record<string, string> {
    if (!this.token) {
      return {};
    }

    return {
      Authorization: `Bearer ${this.token}`,
    };
  }

  public async getResponse(url: string) {
    return await this.request.get(url, { headers: this.getHeaders() });
  }

  public async postResponse(url: string, data: unknown) {
    return await this.request.post(url, { headers: this.getHeaders(), data });
  }

  public async putResponse(url: string, data: unknown) {
    return await this.request.put(url, { headers: this.getHeaders(), data });
  }

  public async deleteReponse(url: string) {
    return await this.request.delete(url, { headers: this.getHeaders() });
  }
}
