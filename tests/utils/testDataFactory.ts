export class TestDataFactory {
  public static uniqueEmail(prefix: string = "playwright"): string {
    return `${prefix}.${Date.now()}@test.com`;
  }

  public static futureDate(daysFromNow: number = 7): string {
    const date = new Date();

    date.setDate(date.getDate() + daysFromNow);

    return date.toISOString();
  }

  public static pastDate(daysAgo: number = 1): string {
    const date = new Date();

    date.setDate(date.getDate() - daysAgo);

    return date.toISOString();
  }

  public static today(): string {
    return new Date().toISOString();
  }

  public static trainee() {
    return {
      firstName: "Playwright",
      lastName: "Test",
      email: this.uniqueEmail("trainee"),
      techStack: "TypeScript",
      status: "Active",
    };
  }

  public static traineeUpdate() {
    return {
      firstName: "Updated",
      lastName: "Test",
      email: this.uniqueEmail("updated.trainee"),
      techStack: "C#",
      status: "Completed",
    };
  }

  public static mentor() {
    return {
      firstName: "Playwright",
      lastName: "Mentor",
      email: this.uniqueEmail("mentor"),
      expertise: "TypeScript",
      status: "Active",
    };
  }

  public static mentorUpdate() {
    return {
      firstName: "Updated",
      lastName: "Mentor",
      email: this.uniqueEmail("updated.mentor"),
      expertise: "C#",
      status: "Active",
    };
  }

  public static learningTask() {
    return {
      title: `Playwright Learning Task ${this.today()}`,
      description: "Learning task created for API testing.",
      expectedTechStack: "TypeScript",
      dueDate: this.futureDate(7),
      status: "Draft",
    };
  }

  public static learningTaskUpdate() {
    return {
      title: `Updated Learning Task ${this.today()}`,
      description: "Learning task updated during API testing.",
      expectedTechStack: "C#",
      dueDate: this.futureDate(14),
      status: "Published",
    };
  }

  public static taskAssignment(
    traineeId: number,
    mentorId: number,
    learningTaskId: number,
  ) {
    return {
      traineeId,
      mentorId,
      learningTaskId,
      dueDate: this.futureDate(7),
      status: "Assigned",
      remarks: "Created by Test",
    };
  }

  public static taskAssignmentUpdate() {
    return {
      status: "InProgress",
    };
  }

  public static submission(taskAssignmentId: number) {
    return {
      taskAssignmentId,
      submissionUrl: "https://test.url.com",
      notes: "Submission created through Playwright API testing.",
      status: "Submitted",
    };
  }

  public static review(submissionId: number, mentorId: number) {
    return {
      submissionId,
      mentorId,
      feedback: "Review created through Playwright API testing.",
      reviewStatus: "Accepted",
    };
  }

  public static invalidEmail(): string {
    return "invalid-email";
  }

  public static invalidStatus(): string {
    return "invalid-status";
  }

  public static invalidDueDate(): string {
    return this.pastDate(1);
  }
}
