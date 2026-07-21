using TraineeManagement.Api.DTOs;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace TraineeManagement.Api.Services
{
    public class LearningTaskService : ILearningTaskService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearningTaskService> _logger;
        public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private LearningTaskResponse MapToResponse(LearningTask learningTask)
        {
            return new LearningTaskResponse
            {
                Id = learningTask.Id,
                Title = learningTask.Title,
                Description = learningTask.Description,
                ExpectedTechStack = learningTask.ExpectedTechStack,
                DueDate = learningTask.DueDate,
                Status = learningTask.Status.ToString()
            };
        }

        private IQueryable<LearningTask> SearchFilterQuery(IQueryable<LearningTask> query, string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(learningTask =>
                    learningTask.Title.ToLower().Contains(searchTermLower) ||
                    learningTask.Description.ToLower().Contains(searchTermLower) ||
                    learningTask.ExpectedTechStack.ToLower().Contains(searchTermLower)
                );
            }
            return query;
        }

        private IQueryable<LearningTask> StatusFilterQuery(IQueryable<LearningTask> query, string? status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(status, true, out LearningTaskStatus parsedStatus))
                {
                    query = query.Where(learningTask => learningTask.Status == parsedStatus);
                }
                else
                {
                    query = query.Where(learningTask => false);
                }
            }
            return query;
        }

        private IQueryable<LearningTask> AllFilterQueries(string? searchTerm, string? status)
        {
            IQueryable<LearningTask> query = _context.LearningTasks.AsQueryable();
            query = SearchFilterQuery(query, searchTerm);
            query = StatusFilterQuery(query, status);
            return query;
        }

        public async Task<PagedResponse<LearningTaskResponse>> GetLearningTasksAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            // Preventing bad input for pageNumber and pageSize 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            IQueryable<LearningTask> query = AllFilterQueries(searchTerm, status);

            int totalRecords = await query.CountAsync();

            List<LearningTask> pagedlearningTasks = await query.OrderBy(learningTask => learningTask.Id)
                           .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            List<LearningTaskResponse> learningTaskResponses = pagedlearningTasks.Select(MapToResponse).ToList();
            return new PagedResponse<LearningTaskResponse>(pageNumber, pageSize, totalRecords, learningTaskResponses);
        }

        public async Task<LearningTaskResponse?> GetLearningTaskByIdAsync(int id)
        {
            LearningTask? learningTask = await _context.LearningTasks.FindAsync(id);
            if (learningTask is null) return null;
            return MapToResponse(learningTask);
        }

        public async Task<LearningTaskResponse> AddLearningTaskAsync(CreateLearningTaskRequest request)
        {
            LearningTask newLearningTask = new()
            {
                Title = request.Title,
                Description = request.Description,
                ExpectedTechStack = request.ExpectedTechStack,
                DueDate = request.DueDate,
                Status = EnumHelper.ParseOrThrow<LearningTaskStatus>(request.Status, nameof(request.Status)),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _context.LearningTasks.AddAsync(newLearningTask);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Learning Task Created Successfully with ID: {taskid} at Timestamp: {createdDate}",
                newLearningTask.Id, newLearningTask.CreatedDate);
            return MapToResponse(newLearningTask);
        }

        public async Task<LearningTaskResponse?> UpdateLearningTaskAsync(int id, UpdateLearningTaskRequest request)
        {
            LearningTask? learningTask = await _context.LearningTasks.FindAsync(id);
            if (learningTask is null) return null;

            learningTask.Title = request.Title;
            learningTask.Description = request.Description;
            learningTask.ExpectedTechStack = request.ExpectedTechStack;
            learningTask.DueDate = request.DueDate;
            learningTask.Status = EnumHelper.ParseOrThrow<LearningTaskStatus>(request.Status, nameof(request.Status));
            learningTask.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Learning Task with ID: {taskId} was successfully updated at Timestamp: {updatedDate} UTC",
            learningTask.Id, learningTask.UpdatedDate);
            return MapToResponse(learningTask);
        }

        public async Task<bool> DeleteLearningTaskAsync(int id)
        {
            LearningTask? learningTask = await _context.LearningTasks.FindAsync(id);
            if (learningTask is null) return false;

            _context.LearningTasks.Remove(learningTask);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Learning Task with ID: {learningTask.Id} was successfully deleted at Timestamp: {DateTime.UtcNow} UTC",
            learningTask.Id, DateTime.UtcNow);
            return true;
        }
    }
}