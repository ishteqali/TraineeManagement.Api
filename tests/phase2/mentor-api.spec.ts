import { test, expect, APIRequestContext, request } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { CleanupHelper } from "../utils/cleanupHelper";

test.describe("Mentor API", () => {
  let context: APIRequestContext;
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeAll(async () => {
    context = await request.newContext();

    const token = await getAdminToken(context);

    api = new ApiClient(context, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all mentors", async () => {
    const response = await api.get("api/mentors");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("totalRecords");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should get mentor by id", async () => {
    const mentorData = TestDataFactory.mentor();

    const createResponse = await api.post("api/mentors", mentorData);

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const response = await api.get(`api/mentors/${mentorId}`);

    expect(response.status()).toBe(200);

    const mentor = await response.json();

    expect(mentor).toBeDefined();
    expect(mentor.id).toBe(mentorId);
    expect(mentor.firstName).toBe(mentorData.firstName);
    expect(mentor.lastName).toBe(mentorData.lastName);
    expect(mentor.email).toBe(mentorData.email);
    expect(mentor.expertise).toBe(mentorData.expertise);
    expect(mentor.status).toBe(mentorData.status);

    await cleanup.deleteMentor(mentorId);
  });

  test("should create a mentor", async () => {
    const mentorData = TestDataFactory.mentor();

    const response = await api.post("api/mentors", mentorData);

    expect(response.status()).toBe(201);

    const mentor = await response.json();

    expect(mentor.id).toBeDefined();
    expect(mentor.firstName).toBe(mentorData.firstName);
    expect(mentor.lastName).toBe(mentorData.lastName);
    expect(mentor.email).toBe(mentorData.email);
    expect(mentor.expertise).toBe(mentorData.expertise);
    expect(mentor.status).toBe(mentorData.status);

    await cleanup.deleteMentor(mentor.id);
  });

  test("should update a mentor", async () => {
    const mentorData = TestDataFactory.mentor();

    const createResponse = await api.post("api/mentors", mentorData);

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const updateData = TestDataFactory.mentor({
      firstName: "Updated",
      lastName: "Mentor",
      expertise: "C#",
      status: "Active",
    });

    const updateResponse = await api.put(`api/mentors/${mentorId}`, updateData);

    expect(updateResponse.status()).toBe(200);

    const updatedMentor = await updateResponse.json();

    expect(updatedMentor.id).toBe(mentorId);
    expect(updatedMentor.firstName).toBe(updateData.firstName);
    expect(updatedMentor.lastName).toBe(updateData.lastName);
    expect(updatedMentor.email).toBe(updateData.email);
    expect(updatedMentor.expertise).toBe(updateData.expertise);
    expect(updatedMentor.status).toBe(updateData.status);

    await cleanup.deleteMentor(mentorId);
  });

  test("should delete a mentor", async () => {
    const mentorData = TestDataFactory.mentor();

    const createResponse = await api.post("api/mentors", mentorData);

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const deleteResponse = await api.delete(`api/mentors/${mentorId}`);

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/mentors/${mentorId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing mentor", async () => {
    const response = await api.get("api/mentors/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing mentor", async () => {
    const mentorData = TestDataFactory.mentor();

    const response = await api.put("api/mentors/999999999", mentorData);

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing mentor", async () => {
    const response = await api.delete("api/mentors/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid mentor data", async () => {
    const invalidMentor = TestDataFactory.mentor({
      firstName: "",
      lastName: "",
      email: TestDataFactory.invalidEmail(),
      expertise: "",
      status: TestDataFactory.invalidStatus(),
    });

    const response = await api.post("api/mentors", invalidMentor);

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const mentorData = TestDataFactory.mentor();

    const response = await api.post("api/mentors", {
      lastName: mentorData.lastName,
      email: mentorData.email,
      expertise: mentorData.expertise,
      status: mentorData.status,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const mentorData = TestDataFactory.mentor({
      email: TestDataFactory.invalidEmail(),
    });

    const response = await api.post("api/mentors", mentorData);

    expect(response.status()).toBe(400);
  });

  test("should search mentors", async () => {
    const uniqueName = `SearchMentor${Date.now()}`;
    const mentorData = TestDataFactory.mentor({
      firstName: uniqueName,
      email: `${uniqueName.toLowerCase()}@test.com`,
    });

    const createResponse = await api.post("api/mentors", mentorData);

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const response = await api.get(
      `api/mentors?pageNumber=1&pageSize=10&search=${encodeURIComponent(
        uniqueName,
      )}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body.data).toBeDefined();
    expect(Array.isArray(body.data)).toBeTruthy();

    expect(
      body.data.some(
        (mentor: { firstName: string }) =>
          mentor.firstName === mentorData.firstName,
      ),
    ).toBeTruthy();

    await cleanup.deleteMentor(mentorId);
  });

  test("should filter mentors by status", async () => {
    const response = await api.get(
      "api/mentors?pageNumber=1&pageSize=10&status=Active",
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/mentors");

    expect(response.status()).toBe(401);
  });

  test.afterAll(async () => {
    await context.dispose();
  });
});
