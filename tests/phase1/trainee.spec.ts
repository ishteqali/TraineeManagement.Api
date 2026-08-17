import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";

test.describe("Trainee API", () => {
  test("Should return 401 when JWT token is not provided", async ({
    request,
  }) => {
    const api = new ApiClient(request);

    const response = await api.getResponse("api/trainees/1");
    expect(response.status()).toBe(401);
  });

  test("Should get trainee by id", async ({ request }) => {
    const token = await getAdminToken(request);
    const api = new ApiClient(request, token);

    const response = await api.getResponse("/api/trainees/1");
    expect(response.status()).toBe(200);

    const trainee = await response.json();
    expect(trainee).toBeDefined();
  });
});
