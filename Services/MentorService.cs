using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TraineeManagement.Api.Services
{
    public class MentorService : IMentorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MentorService> _logger;
        public MentorService(AppDbContext context, ILogger<MentorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private MentorResponse MapToResponse(Mentor mentor)
        {
            return new MentorResponse
            {
                Id = mentor.Id,
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Email = mentor.Email,
                Expertise = mentor.Expertise,
                Status = mentor.Status.ToString()
            };
        }

        private IQueryable<Mentor> SearchFilterQuery(IQueryable<Mentor> query, string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(mentor =>
                    mentor.FirstName.ToLower().Contains(searchTermLower) ||
                    mentor.LastName.ToLower().Contains(searchTermLower) ||
                    mentor.Email.ToLower().Contains(searchTermLower) ||
                    mentor.Expertise.ToLower().Contains(searchTermLower)
                );
            }
            return query;
        }

        private IQueryable<Mentor> StatusFilterQuery(IQueryable<Mentor> query, string? status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(status, true, out MentorStatus parsedStatus))
                {
                    query = query.Where(mentor => mentor.Status == parsedStatus);
                }
                else
                {
                    query = query.Where(mentor => false);
                }
            }
            return query;
        }

        private IQueryable<Mentor> AllFilterQueries(string? searchTerm, string? status)
        {
            IQueryable<Mentor> query = _context.Mentors.AsQueryable();
            query = SearchFilterQuery(query, searchTerm);
            query = StatusFilterQuery(query, status);
            return query;
        }

        public async Task<PagedResponse<MentorResponse>> GetMentorsAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            // Preventing bad input for pageNumber and pageSize 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            IQueryable<Mentor> query = AllFilterQueries(searchTerm, status);

            int totalRecords = await query.CountAsync();

            List<Mentor> pagedMentors = await query.OrderBy(mentor => mentor.Id)
                           .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            List<MentorResponse> mentorResponses = pagedMentors.Select(MapToResponse).ToList();
            return new PagedResponse<MentorResponse>(pageNumber, pageSize, totalRecords, mentorResponses);
        }

        public async Task<MentorResponse?> GetMentorByIdAsync(int id)
        {
            Mentor? mentor = await _context.Mentors.FindAsync(id);
            if (mentor == null) return null;
            return MapToResponse(mentor);
        }

        public async Task<MentorResponse> AddMentorAsync(CreateMentorRequest request)
        {
            Mentor newMentor = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Expertise = request.Expertise,
                Status = Enum.Parse<MentorStatus>(request.Status!.ToString(), ignoreCase: true),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _context.Mentors.AddAsync(newMentor);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Mentor Created Successfully with ID: {newMentor.Id} at Timestamp: {newMentor.CreatedDate}");
            return MapToResponse(newMentor);
        }

        public async Task<MentorResponse?> UpdateMentorAsync(int id, UpdateMentorRequest request)
        {
            Mentor? mentor = await _context.Mentors.FindAsync(id);
            if (mentor == null) return null;

            mentor.FirstName = request.FirstName;
            mentor.LastName = request.LastName;
            mentor.Email = request.Email;
            mentor.Expertise = request.Expertise;
            mentor.Status = Enum.Parse<MentorStatus>(request.Status!.ToString(), ignoreCase: true);
            mentor.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Mentor with ID: {mentor.Id} was successfully updated at Timestamp: {mentor.UpdatedDate} UTC");
            return MapToResponse(mentor);
        }

        public async Task<bool> DeleteMentorAsync(int id)
        {
            Mentor? mentor = await _context.Mentors.FindAsync(id);
            if (mentor == null) return false;

            _context.Mentors.Remove(mentor);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Mentor with ID: {mentor.Id} was successfully deleted at Timestamp: {DateTime.UtcNow} UTC");
            return true;
        }
    }
}