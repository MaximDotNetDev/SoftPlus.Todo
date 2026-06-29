namespace SoftPlus.Todo.Interfaces.Entities;

public class SubStep
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public int TaskId { get; set; }
    public TodoTask? Task { get; set; }
}