import { test, expect, APIRequestContext, request } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Trainee API", () => {
  let context: APIRequestContext;
  let api: ApiClient;

  test.beforeAll(async () => {
    context = await request.newContext();
    const token = await getAdminToken(context);
    api = new ApiClient(context, token);
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
    const traineeData = TestDataFactory.trainee();

    const createResponse = await api.post("api/trainees", traineeData);

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
    const traineeData = TestDataFactory.trainee();

    const response = await api.post("api/trainees", traineeData);

    expect(response.status()).toBe(201);

    const trainee = await response.json();

    expect(trainee.id).toBeDefined();
    expect(trainee.firstName).toBe("Playwright");
    expect(trainee.lastName).toBe("Test");
    expect(trainee.techStack).toBe("TypeScript");
    expect(trainee.status).toBe("Active");
  });

  test("should update a trainee", async () => {
    const traineeData = TestDataFactory.trainee();
    const createResponse = await api.post("api/trainees", traineeData);

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const updatedTraineeData = TestDataFactory.traineeUpdate();
    const updateResponse = await api.put(
      `api/trainees/${traineeId}`,
      updatedTraineeData,
    );

    expect(updateResponse.status()).toBe(200);

    const updatedTrainee = await updateResponse.json();

    expect(updatedTrainee.id).toBe(traineeId);
    expect(updatedTrainee.firstName).toBe("Updated");
    expect(updatedTrainee.lastName).toBe("Test");
    expect(updatedTrainee.techStack).toBe("C#");
    expect(updatedTrainee.status).toBe("Completed");
  });

  test("should delete a trainee", async () => {
    const traineeData = TestDataFactory.trainee();
    const createResponse = await api.post("api/trainees", traineeData);

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
    const updatedTraineeData = TestDataFactory.traineeUpdate();
    const response = await api.put(
      "api/trainees/999999999",
      updatedTraineeData,
    );

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
      email: TestDataFactory.invalidEmail,
      techStack: "",
      status: TestDataFactory.invalidStatus,
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

  test("should filter trainees by status", async () => {
    const response = await api.get(
      "api/trainees?pageNumber=1&pageSize=10&status=Active",
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/trainees");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
