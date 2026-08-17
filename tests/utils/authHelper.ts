import { APIRequestContext, expect } from "@playwright/test";

export async function getAdminToken(
  request: APIRequestContext,
): Promise<string> {
  const response = await request.post("/api/auth/login", {
    data: {
      username: process.env.ADMIN_USERNAME,
      password: process.env.ADMIN_PASSWORD,
    },
  });

  expect(response.status()).toBe(200);

  const body = await response.json();

  expect(body.token).toBeTruthy();

  return body.token;
}
