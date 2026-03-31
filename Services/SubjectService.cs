using System.Security.Claims;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

public class SubjectService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly List<Subject> _subjects = new();
    private static int _nextId = 1;

    public SubjectService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? "demo-user";
    }

    public Task<List<Subject>> GetSubjectsByUserAsync()
    {
        var userId = GetCurrentUserId();

        var userSubjects = _subjects
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Name)
            .ToList();

        return Task.FromResult(userSubjects);
    }

    public Task<Subject?> GetSubjectByIdAsync(int subjectId)
    {
        var userId = GetCurrentUserId();

        var subject = _subjects.FirstOrDefault(s => s.Id == subjectId && s.UserId == userId);

        return Task.FromResult(subject);
    }

    public Task AddSubjectAsync(Subject subject)
    {
        var userId = GetCurrentUserId();

        subject.Id = _nextId++;
        subject.UserId = userId;

        _subjects.Add(subject);

        return Task.CompletedTask;
    }

    public Task UpdateSubjectAsync(Subject updatedSubject)
    {
        var userId = GetCurrentUserId();

        var existingSubject = _subjects.FirstOrDefault(s => s.Id == updatedSubject.Id && s.UserId == userId);

        if (existingSubject is null)
            return Task.CompletedTask;

        existingSubject.Name = updatedSubject.Name;

        return Task.CompletedTask;
    }

    public Task DeleteSubjectAsync(int subjectId)
    {
        var userId = GetCurrentUserId();

        var subject = _subjects.FirstOrDefault(s => s.Id == subjectId && s.UserId == userId);

        if (subject is not null)
        {
            _subjects.Remove(subject);
        }

        return Task.CompletedTask;
    }
}