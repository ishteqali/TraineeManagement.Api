using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace TraineeManagement.Api.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskAssignmentService> _logger;

        public TaskAssignmentService(AppDbContext context, ILogger<TaskAssignmentService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public TaskAssignmentResponse MapToResponse(TaskAssignment taskAssignment)
        {
            return new TaskAssignmentResponse
            {
                Id = taskAssignment.Id,
                TraineeId = taskAssignment.TraineeId,
                TraineeName = $"{taskAssignment.Trainee.FirstName} {taskAssignment.Trainee.LastName}",
                MentorId = taskAssignment.MentorId,
                MentorName = $"{taskAssignment.Mentor.FirstName} {taskAssignment.Mentor.LastName}",
                LearningTaskId = taskAssignment.LearningTaskId,
                TaskTitle = taskAssignment.LearningTask.Title,
                AssignedDate = taskAssignment.AssignedDate,
                DueDate = taskAssignment.DueDate,
                Status = taskAssignment.Status.ToString(),
                Remarks = taskAssignment.Remarks
            };
        }

        public async Task<TaskAssignmentResponse> CreateAssignmentAsync(CreateTaskAssignmentRequest request)
        {
            // Checking Trainee, Mentor and Task Exists or not
            Trainee? trainee = await _context.Trainees.FindAsync(request.TraineeId);
            if (trainee == null) throw new ArgumentException($"Trainee with ID: {request.TraineeId} not exists");

            Mentor? mentor = await _context.Mentors.FindAsync(request.MentorId);
            if (mentor == null) throw new ArgumentException($"Mentor with ID: {request.TraineeId} not exists");

            LearningTask? learningTask = await _context.LearningTasks.FindAsync(request.LearningTaskId);
            if (learningTask == null) throw new ArgumentException($"Trainee with ID: {request.TraineeId} not exists");

            TaskAssignment newTaskAssignment = new TaskAssignment
            {
                TraineeId = request.TraineeId,
                MentorId = request.MentorId,
                LearningTaskId = request.LearningTaskId,
                AssignedDate = request.AssignedDate,
                DueDate = request.DueDate,
                Status = Enum.Parse<TaskAssignmentStatus>(request.Status!.ToString(), ignoreCase: true),
                Remarks = request.Remarks,
                Trainee = trainee,
                Mentor = mentor,
                LearningTask = learningTask
            };
            _context.TaskAssignments.Add(newTaskAssignment);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Task Assignment Created Successfully with ID: {newTaskAssignment.Id} at Timestamp: {DateTime.UtcNow}");
            return MapToResponse(newTaskAssignment);
        }

        public async Task<IEnumerable<TaskAssignmentResponse>> GetAllAssignmentAsnyc()
        {
            IEnumerable<TaskAssignment> taskAssignments = await _context.TaskAssignments
                .Include(taskAssignment => taskAssignment.Trainee)
                .Include(taskAssignment => taskAssignment.Mentor)
                .Include(taskAssignment => taskAssignment.LearningTask).ToListAsync();

            IEnumerable<TaskAssignmentResponse> taskAssignmentResponses = taskAssignments.Select(MapToResponse).ToList();
            return taskAssignmentResponses;
        }

        public async Task<TaskAssignmentResponse?> GetAssignmentByIdAsync(int id)
        {
            TaskAssignment? taskAssignment = await _context.TaskAssignments
                .Include(task => task.Trainee)
                .Include(task => task.Mentor)
                .Include(task => task.LearningTask)
                .FirstOrDefaultAsync(task => task.Id == id);

            if (taskAssignment == null)
            {
                return null;
            }
            return MapToResponse(taskAssignment);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(id);
            if (taskAssignment == null)
            {
                return false;
            }

            taskAssignment.Status = Enum.Parse<TaskAssignmentStatus>(status, ignoreCase: true);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Task Assignment status with ID: {id} was successfully updated at Timestamp: {DateTime.UtcNow} UTC");
            return true;
        }
    }
}