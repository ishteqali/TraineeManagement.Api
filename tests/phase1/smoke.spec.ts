import { test, expect } from "@playwright/test";
import { request } from "node:http";

test("Trainee Management API is reachable", async ({ request }) => {
  const response = await request.get("health/live");

  expect(response.status()).toBe(200);
});
