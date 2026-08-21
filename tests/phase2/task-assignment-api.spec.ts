import { test, expect, APIRequestContext, request } from "@playwright/test";

import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Task Assignment API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();

    const token = await getAdminToken(context);
    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all task assignments", async () => {
    const response = await api.get("api/task-assignments");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should get task assignment by id", async () => {
    const traineeData = TestDataFactory.trainee();
    const traineeResponse = await api.post("api/trainees", traineeData);
    
    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorData = TestDataFactory.mentor();
    const mentorResponse = await api.post("api/mentors", mentorData);

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const learningTaskData = TestDataFactory.learningTask();
    const learningTaskResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(learningTaskResponse.status()).toBe(201);

    const learningTask = await learningTaskResponse.json();

    const assignmentData = TestDataFactory.taskAssignment(
      trainee.id,
      mentor.id,
      learningTask.id,
    );
    const createResponse = await api.post(
      "api/task-assignments",
      assignmentData,
    );

    expect(createResponse.status()).toBe(201);

    const assignment = await createResponse.json();
    const assignmentId = assignment.id;

    const response = await api.get(`api/task-assignments/${assignmentId}`);

    expect(response.status()).toBe(200);

    const result = await response.json();

    expect(result).toBeDefined();
    expect(result.id).toBe(assignmentId);
    expect(result.traineeId).toBe(trainee.id);
    expect(result.mentorId).toBe(mentor.id);
    expect(result.learningTaskId).toBe(learningTask.id);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should create a task assignment", async () => {
    const traineeData = TestDataFactory.trainee();
    const traineeResponse = await api.post("api/trainees", traineeData);

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorData = TestDataFactory.mentor();
    const mentorResponse = await api.post("api/mentors", mentorData);

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const learningTaskData = TestDataFactory.learningTask();
    const learningTaskResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(learningTaskResponse.status()).toBe(201);

    const learningTask = await learningTaskResponse.json();

    const assignmentData = TestDataFactory.taskAssignment(
      trainee.id,
      mentor.id,
      learningTask.id,
    );
    const response = await api.post("api/task-assignments", assignmentData);

    expect(response.status()).toBe(201);

    const assignment = await response.json();

    expect(assignment.id).toBeDefined();
    expect(assignment.traineeId).toBe(trainee.id);
    expect(assignment.mentorId).toBe(mentor.id);
    expect(assignment.learningTaskId).toBe(learningTask.id);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should update task assignment status", async () => {
    const traineeData = TestDataFactory.trainee();
    const traineeResponse = await api.post("api/trainees", traineeData);

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorData = TestDataFactory.mentor();
    const mentorResponse = await api.post("api/mentors", mentorData);

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const learningTaskData = TestDataFactory.learningTask();
    const learningTaskResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(learningTaskResponse.status()).toBe(201);

    const learningTask = await learningTaskResponse.json();

    const assignmentData = TestDataFactory.taskAssignment(
      trainee.id,
      mentor.id,
      learningTask.id,
    );
    const createResponse = await api.post(
      "api/task-assignments",
      assignmentData,
    );

    expect(createResponse.status()).toBe(201);

    const assignment = await createResponse.json();

    const assignmentId = assignment.id;

    const updateData = TestDataFactory.taskAssignmentUpdate();
    const response = await api.put(
      `api/task-assignments/${assignmentId}/status`,
      updateData,
    );
    
    expect(response.status()).toBe(200);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should return 404 for non-existing task assignment", async () => {
    const response = await api.get("api/task-assignments/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing task assignment", async () => {
    const updateData = TestDataFactory.taskAssignmentUpdate();
    const response = await api.put(
      "api/task-assignments/999999999/status",
      updateData,
    );

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid task assignment data", async () => {
    const invalidAssignment = {
      traineeId: 999999999,
      mentorId: 999999999,
      learningTaskId: 999999999,
      dueDate: TestDataFactory.invalidDueDate(),
      status: TestDataFactory.invalidStatus(),
      remarks: "",
    };

    const response = await api.post("api/task-assignments", invalidAssignment);

    expect(response.status()).toBe(400);
  });

  test("should return 400 when trainee id is missing", async () => {
    const mentorData = TestDataFactory.mentor();

    const learningTaskData = TestDataFactory.learningTask();

    const traineeData = TestDataFactory.trainee();

    const traineeResponse = await api.post("api/trainees", traineeData);

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorResponse = await api.post("api/mentors", mentorData);

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const learningTaskResponse = await api.post(
      "api/learning-tasks",
      learningTaskData,
    );

    expect(learningTaskResponse.status()).toBe(201);

    const learningTask = await learningTaskResponse.json();

    const response = await api.post("api/task-assignments", {
      mentorId: mentor.id,
      learningTaskId: learningTask.id,
      dueDate: TestDataFactory.futureDate(7),
      status: "Assigned",
      remarks: "Test",
    });

    expect(response.status()).toBe(400);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should return 400 for invalid due date", async () => {
    const response = await api.post("api/task-assignments", {
      traineeId: 999999999,
      mentorId: 999999999,
      learningTaskId: 999999999,
      dueDate: TestDataFactory.invalidDueDate(),
      status: "Assigned",
      remarks: "Test",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid status update", async () => {
    const response = await api.put("api/task-assignments/999999999/status", {
      status: TestDataFactory.invalidStatus(),
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/task-assignments");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
