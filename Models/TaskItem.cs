namespace SmartStudyPlanner.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public DateTime Deadline { get; set; }

        public bool IsCompleted { get; set; }

        public int SubjectId { get; set; }

        public int UserId { get; set; }

        public Subject? Subject { get; set; }
    }
}