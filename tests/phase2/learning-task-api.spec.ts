import { test, expect, APIRequestContext, request } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Learning Task API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();

    const token = await getAdminToken(context);

    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all learning tasks", async () => {
    const response = await api.get("api/learning-tasks");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("totalRecords");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should get learning task by id", async () => {
    const learningTaskData = TestDataFactory.learningTask();

    const createResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(createResponse.status()).toBe(201);

    const createdLearningTask = await createResponse.json();
    const learningTaskId = createdLearningTask.id;

    const response = await api.get(`api/learning-tasks/${learningTaskId}`);

    expect(response.status()).toBe(200);

    const learningTask = await response.json();

    expect(learningTask).toBeDefined();
    expect(learningTask.id).toBe(learningTaskId);
    expect(learningTask.title).toBe(learningTaskData.title);
    expect(learningTask.description).toBe(learningTaskData.description);
    expect(learningTask.expectedTechStack).toBe(
      learningTaskData.expectedTechStack,
    );
    expect(learningTask.status).toBe(learningTaskData.status);

    await cleanup.deleteLearningTask(learningTaskId);
  });

  test("should create a learning task", async () => {
    const learningTaskData = TestDataFactory.learningTask();

    const response = await api.post("api/learning-tasks", learningTaskData);

    expect(response.status()).toBe(201);

    const learningTask = await response.json();

    expect(learningTask.id).toBeDefined();
    expect(learningTask.title).toBe(learningTaskData.title);
    expect(learningTask.description).toBe(learningTaskData.description);
    expect(learningTask.expectedTechStack).toBe(
      learningTaskData.expectedTechStack,
    );
    expect(learningTask.status).toBe(learningTaskData.status);

    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should update a learning task", async () => {
    const learningTaskData = TestDataFactory.learningTask();
    const createResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(createResponse.status()).toBe(201);

    const createdLearningTask = await createResponse.json();
    const learningTaskId = createdLearningTask.id;

    const updatedLearningTaskData = TestDataFactory.learningTask({
      title: "Updating Playwright Learning Task",
      description: "Updating this Learning Task",
      dueDate: TestDataFactory.futureDate(8),
      status: "Published",
    });

    const updateResponse = await api.put(
      `api/learning-tasks/${learningTaskId}`,
      updatedLearningTaskData,
    );

    expect(updateResponse.status()).toBe(200);

    const updatedLearningTask = await updateResponse.json();

    expect(updatedLearningTask.id).toBe(learningTaskId);
    expect(updatedLearningTask.title).toBe(updatedLearningTaskData.title);
    expect(updatedLearningTask.description).toBe(
      updatedLearningTaskData.description,
    );
    expect(updatedLearningTask.dueDate).toBe(updatedLearningTaskData.dueDate);
    expect(updatedLearningTask.status).toBe(updatedLearningTaskData.status);

    await cleanup.deleteLearningTask(learningTaskId);
  });

  test("should delete a learning task", async () => {
    const learningTaskData = TestDataFactory.learningTask();

    const createResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(createResponse.status()).toBe(201);

    const createdLearningTask = await createResponse.json();
    const learningTaskId = createdLearningTask.id;

    const deleteResponse = await api.delete(
      `api/learning-tasks/${learningTaskId}`,
    );

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/learning-tasks/${learningTaskId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing learning task", async () => {
    const response = await api.get("api/learning-tasks/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing learning task", async () => {
    const learningTaskData = TestDataFactory.learningTask({
      status: "Closed",
    });

    const response = await api.put(
      "api/learning-tasks/999999999",
      learningTaskData,
    );

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing learning task", async () => {
    const response = await api.delete("api/learning-tasks/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid learning task data", async () => {
    const invalidLearningTask = TestDataFactory.learningTask({
      title: "",
      description: "",
      expectedTechStack: "",
      status: TestDataFactory.invalidStatus(),
    });

    const response = await api.post("api/learning-tasks", invalidLearningTask);

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when title is missing", async () => {
    const learningTaskData = TestDataFactory.learningTask();

    const response = await api.post("api/learning-tasks", {
      description: learningTaskData.description,
      expectedTechStack: learningTaskData.expectedTechStack,
      dueDate: learningTaskData.dueDate,
      status: learningTaskData.status,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid due date", async () => {
    const invalidLearningTask = TestDataFactory.learningTask({
      dueDate: TestDataFactory.invalidDueDate(),
    });

    const response = await api.post("api/learning-tasks", invalidLearningTask);

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid status", async () => {
    const invalidLearningTask = TestDataFactory.learningTask({
      status: TestDataFactory.invalidStatus(),
    });

    const response = await api.post("api/learning-tasks", invalidLearningTask);

    expect(response.status()).toBe(400);
  });

  test("should search learning tasks", async () => {
    const uniqueTitle = `Search${Date.now()}`;

    const learningTaskData = TestDataFactory.learningTask({
      title: uniqueTitle,
    });

    const createResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(createResponse.status()).toBe(201);

    const createdLearningTask = await createResponse.json();
    const learningTaskId = createdLearningTask.id;

    const response = await api.get(
      `api/learning-tasks?pageNumber=1&pageSize=10&search=${encodeURIComponent(
        uniqueTitle,
      )}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body.data).toBeDefined();
    expect(Array.isArray(body.data)).toBeTruthy();

    expect(
      body.data.some(
        (learningTask: { title: string }) => learningTask.title === uniqueTitle,
      ),
    ).toBeTruthy();

    await cleanup.deleteLearningTask(learningTaskId);
  });

  test("should filter learning tasks by status", async () => {
    const response = await api.get(
      "api/learning-tasks?pageNumber=1&pageSize=10&status=Draft",
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/learning-tasks");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
