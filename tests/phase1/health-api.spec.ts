import { test, expect } from "@playwright/test";

test.describe("Health Check API", () => {
  test("should return healthy liveness response", async ({ request }) => {
    const response = await request.get("/health/live");

    expect(response.status()).toBe(200);
  });
});
