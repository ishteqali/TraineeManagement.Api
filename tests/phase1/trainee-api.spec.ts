import { test, expect, APIRequestContext, request } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Trainee API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();
    const token = await getAdminToken(context);
    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
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
    expect(trainee.firstName).toBe(traineeData.firstName);
    expect(trainee.lastName).toBe(traineeData.lastName);
    expect(trainee.techStack).toBe(traineeData.techStack);
    expect(trainee.status).toBe(traineeData.status);

    await cleanup.deleteTrainee(traineeId);
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

    await cleanup.deleteTrainee(trainee.id);
  });

  test("should update a trainee", async () => {
    const traineeData = TestDataFactory.trainee();
    const createResponse = await api.post("api/trainees", traineeData);

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const updatedTraineeData = TestDataFactory.trainee({
      firstName: "Updated",
      techStack: "C#",
      status: "Completed",
    });
    const updateResponse = await api.put(
      `api/trainees/${traineeId}`,
      updatedTraineeData,
    );

    expect(updateResponse.status()).toBe(200);

    const updatedTrainee = await updateResponse.json();

    expect(updatedTrainee.id).toBe(traineeId);
    expect(updatedTrainee.firstName).toBe(updatedTraineeData.firstName);
    expect(updatedTrainee.lastName).toBe(updatedTraineeData.lastName);
    expect(updatedTrainee.techStack).toBe(updatedTraineeData.techStack);
    expect(updatedTrainee.status).toBe(updatedTraineeData.status);

    await cleanup.deleteTrainee(traineeId);
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
    const updatedTraineeData = TestDataFactory.trainee({
      firstName: "Updated",
      techStack: "C#",
      status: "Completed",
    });
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
    const invalidTrainee = TestDataFactory.trainee({
      firstName: "",
      lastName: "",
      email: TestDataFactory.invalidEmail(),
      techStack: "",
      status: TestDataFactory.invalidStatus(),
    });
    const response = await api.post("api/trainees", invalidTrainee);

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const traineeData = TestDataFactory.trainee();

    const response = await api.post("api/trainees", {
      lastName: traineeData.lastName,
      email: traineeData.email,
      techStack: traineeData.techStack,
      status: traineeData.status,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const invalidEmailTrainee = TestDataFactory.trainee({
      email: TestDataFactory.invalidEmail(),
    });
    const response = await api.post("api/trainees", invalidEmailTrainee);

    expect(response.status()).toBe(400);
  });

  test("should search trainees", async () => {
    const uniqueName = `Search${Date.now()}`;
    const searchTrainee = TestDataFactory.trainee({
      firstName: uniqueName,
      email: `${uniqueName.toLowerCase()}@test.com`,
    });

    const createResponse = await api.post("api/trainees", searchTrainee);

    expect(createResponse.status()).toBe(201);

    const createdData = await createResponse.json();
    const traineeId = createdData.id;

    const response = await api.get(
      `api/trainees?pageNumber=1&pageSize=10&search=${encodeURIComponent(uniqueName)}`,
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

    await cleanup.deleteTrainee(traineeId);
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
