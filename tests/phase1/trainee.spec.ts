import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";

test.describe("Trainee API", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should get all trainees", async () => {
    const response = await api.get("api/trainees");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("totalRecords");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should get trainee by id", async () => {
    const email = `playwright.${Date.now()}@test.com`;

    const createResponse = await api.post("api/trainees", {
      firstName: "Playwright",
      lastName: "Test",
      email,
      techStack: "TypeScript",
      status: "Active",
    });

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const response = await api.get(`api/trainees/${traineeId}`);

    expect(response.status()).toBe(200);

    const trainee = await response.json();

    expect(trainee).toBeDefined();
    expect(trainee.id).toBe(traineeId);
    expect(trainee.firstName).toBe("Playwright");
    expect(trainee.lastName).toBe("Test");
    expect(trainee.techStack).toBe("TypeScript");
    expect(trainee.status).toBe("Active");
  });

  test("should create a trainee", async () => {
    const email = `playwright.${Date.now()}@test.com`;

    const response = await api.post("api/trainees", {
      firstName: "Playwright",
      lastName: "Create",
      email,
      techStack: "TypeScript",
      status: "Active",
    });

    expect(response.status()).toBe(201);

    const trainee = await response.json();

    expect(trainee.id).toBeDefined();
    expect(trainee.firstName).toBe("Playwright");
    expect(trainee.lastName).toBe("Create");
    expect(trainee.email).toBe(email);
    expect(trainee.techStack).toBe("TypeScript");
    expect(trainee.status).toBe("Active");
  });

  test("should update a trainee", async () => {
    const createResponse = await api.post("api/trainees", {
      firstName: "Before",
      lastName: "Update",
      email: `update.${Date.now()}@test.com`,
      techStack: "C#",
      status: "Active",
    });

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const updateResponse = await api.put(`api/trainees/${traineeId}`, {
      firstName: "After",
      lastName: "Update",
      email: `updated.${Date.now()}@test.com`,
      techStack: "TypeScript",
      status: "Completed",
    });

    expect(updateResponse.status()).toBe(200);

    const updatedTrainee = await updateResponse.json();

    expect(updatedTrainee.id).toBe(traineeId);
    expect(updatedTrainee.firstName).toBe("After");
    expect(updatedTrainee.lastName).toBe("Update");
    expect(updatedTrainee.techStack).toBe("TypeScript");
    expect(updatedTrainee.status).toBe("Completed");
  });

  test("should delete a trainee", async () => {
    const createResponse = await api.post("api/trainees", {
      firstName: "Delete",
      lastName: "Test",
      email: `delete.${Date.now()}@test.com`,
      techStack: "C#",
      status: "Inactive",
    });

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const deleteResponse = await api.delete(`api/trainees/${traineeId}`);

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/trainees/${traineeId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing trainee", async () => {
    const response = await api.get("api/trainees/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing trainee", async () => {
    const response = await api.put("api/trainees/999999999", {
      firstName: "Test",
      lastName: "Update",
      email: `notfound.${Date.now()}@test.com`,
      techStack: "TypeScript",
      status: "Active",
    });

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing trainee", async () => {
    const response = await api.delete("api/trainees/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid trainee data", async () => {
    const response = await api.post("api/trainees", {
      firstName: "",
      lastName: "",
      email: "invalid-email",
      techStack: "",
      status: "InvalidStatus",
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const response = await api.post("api/trainees", {
      lastName: "Test",
      email: `missing.${Date.now()}@test.com`,
      techStack: "TypeScript",
      status: "Active",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const response = await api.post("api/trainees", {
      firstName: "Invalid",
      lastName: "Email",
      email: "invalid-email",
      techStack: "TypeScript",
      status: "Active",
    });

    expect(response.status()).toBe(400);
  });

  test("should search trainees", async () => {
    const uniqueName = `Search${Date.now()}`;

    const createResponse = await api.post("api/trainees", {
      firstName: uniqueName,
      lastName: "Trainee",
      email: `${uniqueName.toLowerCase()}@test.com`,
      techStack: "TypeScript",
      status: "Active",
    });

    expect(createResponse.status()).toBe(201);

    const response = await api.get(
      `api/trainees?pageNumber=1&pageSize=10&search=${uniqueName}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body.data).toBeDefined();
    expect(Array.isArray(body.data)).toBeTruthy();

    expect(
      body.data.some(
        (trainee: { firstName: string }) => trainee.firstName === uniqueName,
      ),
    ).toBeTruthy();
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/trainees");

    expect(response.status()).toBe(401);
  });
});
