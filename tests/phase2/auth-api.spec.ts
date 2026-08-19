import { test, expect, request, APIRequestContext } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";

test.describe("Authentication Api", () => {
  let context: APIRequestContext;

  test.beforeAll(async () => {
    context = await request.newContext();
  });

  test("should login successfully with valid credentials", async () => {
    const response = await context.post("api/auth/login", {
      data: {
        username: process.env.ADMIN_USERNAME,
        password: process.env.ADMIN_PASSWORD,
      },
    });

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(body.token).toBeTruthy();
    expect(body.user).toBeDefined();

    expect(body.user.username).toBe(process.env.ADMIN_USERNAME);

    expect(body).not.toHaveProperty("passwordHash");
    expect(body.user).not.toHaveProperty("passwordHash");
  });

  test("should reject invalid credentials", async () => {
    const response = await context.post("api/auth/login", {
      data: {
        username: process.env.ADMIN_USERNAME,
        password: "WrongPassword123!",
      },
    });

    expect(response.status()).toBe(401);
  });

  test("should reject missing credentials", async () => {
    const response = await context.post("api/auth/login", {
      data: {
        username: "",
        password: "",
      },
    });

    expect(response.status()).toBe(400);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
