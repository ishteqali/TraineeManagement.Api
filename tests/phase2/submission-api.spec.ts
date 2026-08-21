import { test, expect, APIRequestContext, request } from "@playwright/test";

import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Submission API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();

    const token = await getAdminToken(context);

    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all submissions", async () => {
    const response = await api.get("api/submissions");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should get submission by id", async () => {
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
    const assignmentResponse = await api.post(
      "api/task-assignments",
      assignmentData,
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    const submissionData = TestDataFactory.submission(assignment.id);
    const createResponse = await api.post("api/submissions", submissionData);

    expect(createResponse.status()).toBe(201);

    const submission = await createResponse.json();
    const submissionId = submission.id;

    const response = await api.get(`api/submissions/${submissionId}`);

    expect(response.status()).toBe(200);

    const result = await response.json();

    expect(result).toBeDefined();
    expect(result.id).toBe(submissionId);
    expect(result.taskAssignmentId).toBe(assignment.id);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should create a submission", async () => {
    const traineeResponse = await api.post(
      "api/trainees",
      TestDataFactory.trainee(),
    );

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorResponse = await api.post(
      "api/mentors",
      TestDataFactory.mentor(),
    );

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const learningTaskResponse = await api.post(
      "api/learning-tasks",
      TestDataFactory.learningTask(),
    );

    expect(learningTaskResponse.status()).toBe(201);

    const learningTask = await learningTaskResponse.json();

    const assignmentResponse = await api.post(
      "api/task-assignments",
      TestDataFactory.taskAssignment(trainee.id, mentor.id, learningTask.id),
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();
    const submissionData = TestDataFactory.submission(assignment.id);
    const response = await api.post("api/submissions", submissionData);

    expect(response.status()).toBe(201);

    const submission = await response.json();

    expect(submission.id).toBeDefined();
    expect(submission.taskAssignmentId).toBe(assignment.id);
    expect(submission.submissionUrl).toBe(submissionData.submissionUrl);
    expect(submission.notes).toBe(submissionData.notes);
    expect(submission.status).toBe(submissionData.status);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should return 404 for non-existing submission", async () => {
    const response = await api.get("api/submissions/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid submission data", async () => {
    const invalidSubmission = {
      taskAssignmentId: 999999999,
      submissionUrl: "invalid-url",
      notes: "",
      status: TestDataFactory.invalidStatus(),
    };
    const response = await api.post("api/submissions", invalidSubmission);

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when task assignment id is missing", async () => {
    const response = await api.post("api/submissions", {
      submissionUrl: "https://test.url.com",
      notes: "Submission test",
      status: "Submitted",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid submission URL", async () => {
    const response = await api.post("api/submissions", {
      taskAssignmentId: 999999999,
      submissionUrl: "invalid-url",
      notes: "Submission test",
      status: "Submitted",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/submissions");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
