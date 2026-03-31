using System.Security.Claims;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

public class TaskService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly List<StudyTask> _tasks = new();
    private static int _nextId = 1;

    public TaskService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? "demo-user";
    }

    public Task<List<StudyTask>> GetTasksByUserAsync()
    {
        var userId = GetCurrentUserId();

        var userTasks = _tasks
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.DueDate)
            .ToList();

        return Task.FromResult(userTasks);
    }

    public Task<List<StudyTask>> GetTasksBySubjectAsync(int subjectId)
    {
        var userId = GetCurrentUserId();

        var filteredTasks = _tasks
            .Where(t => t.UserId == userId && t.SubjectId == subjectId)
            .OrderBy(t => t.DueDate)
            .ToList();

        return Task.FromResult(filteredTasks);
    }

    public Task<StudyTask?> GetTaskByIdAsync(int taskId)
    {
        var userId = GetCurrentUserId();

        var task = _tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);

        return Task.FromResult(task);
    }

    public Task AddTaskAsync(StudyTask task)
    {
        var userId = GetCurrentUserId();

        task.Id = _nextId++;
        task.UserId = userId;

        _tasks.Add(task);

        return Task.CompletedTask;
    }

    public Task UpdateTaskAsync(StudyTask updatedTask)
    {
        var userId = GetCurrentUserId();

        var existingTask = _tasks.FirstOrDefault(t => t.Id == updatedTask.Id && t.UserId == userId);

        if (existingTask is null)
            return Task.CompletedTask;

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.IsCompleted = updatedTask.IsCompleted;
        existingTask.SubjectId = updatedTask.SubjectId;

        return Task.CompletedTask;
    }

    public Task DeleteTaskAsync(int taskId)
    {
        var userId = GetCurrentUserId();

        var task = _tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);

        if (task is not null)
        {
            _tasks.Remove(task);
        }

        return Task.CompletedTask;
    }
}