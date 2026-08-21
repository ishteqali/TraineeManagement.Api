import { test, expect, APIRequestContext, request } from "@playwright/test";

import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Review API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();

    const token = await getAdminToken(context);

    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all reviews", async () => {
    const response = await api.get("api/reviews");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should get review by id", async () => {
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
    const submissionResponse = await api.post(
      "api/submissions",
      submissionData,
    );

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    const reviewData = TestDataFactory.review(submission.id, mentor.id);
    const createResponse = await api.post("api/reviews", reviewData);

    expect(createResponse.status()).toBe(201);

    const review = await createResponse.json();

    const reviewId = review.id;

    const response = await api.get(`api/reviews/${reviewId}`);

    expect(response.status()).toBe(200);

    const result = await response.json();

    expect(result).toBeDefined();
    expect(result.id).toBe(reviewId);
    expect(result.submissionId).toBe(submission.id);
    expect(result.mentorId).toBe(mentor.id);
    expect(result.feedback).toBe(reviewData.feedback);
    expect(result.reviewStatus).toBe(reviewData.reviewStatus);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should create a review", async () => {
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

    const submissionResponse = await api.post(
      "api/submissions",
      TestDataFactory.submission(assignment.id),
    );

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    const reviewData = TestDataFactory.review(submission.id, mentor.id);
    const response = await api.post("api/reviews", reviewData);

    expect(response.status()).toBe(201);

    const review = await response.json();

    expect(review.id).toBeDefined();
    expect(review.submissionId).toBe(submission.id);
    expect(review.mentorId).toBe(mentor.id);
    expect(review.feedback).toBe(reviewData.feedback);
    expect(review.reviewStatus).toBe(reviewData.reviewStatus);

    await cleanup.deleteTrainee(trainee.id);
    await cleanup.deleteMentor(mentor.id);
    await cleanup.deleteLearningTask(learningTask.id);
  });

  test("should return 404 for non-existing review", async () => {
    const response = await api.get("api/reviews/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid review data", async () => {
    const invalidReview = {
      submissionId: 999999999,
      mentorId: 999999999,
      feedback: "",
      reviewStatus: "InvalidStatus",
    };

    const response = await api.post("api/reviews", invalidReview);

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when submission id is missing", async () => {
    const response = await api.post("api/reviews", {
      mentorId: 999999999,
      feedback: "Review created through Playwright testing.",
      reviewStatus: "Accepted",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 when mentor id is missing", async () => {
    const response = await api.post("api/reviews", {
      submissionId: 999999999,
      feedback: "Review created through Playwright testing.",
      reviewStatus: "Accepted",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid review status", async () => {
    const response = await api.post("api/reviews", {
      submissionId: 999999999,
      mentorId: 999999999,
      feedback: "Review created through Playwright testing.",
      reviewStatus: "InvalidStatus",
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/reviews");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
