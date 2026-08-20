import { ApiClient } from "./apiClient";

export class CleanupHelper {
  private readonly api: ApiClient;

  constructor(api: ApiClient) {
    this.api = api;
  }

  public async deleteTrainee(traineeId: number): Promise<void> {
    const response = await this.api.delete(`api/trainees/${traineeId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup trainee ${traineeId}. Status: ${response.status()}`,
      );
    }
  }

  public async deleteMentor(mentorId: number): Promise<void> {
    const response = await this.api.delete(`api/mentors/${mentorId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup mentor ${mentorId}. Status: ${response.status()}`,
      );
    }
  }

  public async deleteLearningTask(taskId: number): Promise<void> {
    const response = await this.api.delete(`api/learning-tasks/${taskId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup learning task ${taskId}. Status: ${response.status()}`,
      );
    }
  }
}
