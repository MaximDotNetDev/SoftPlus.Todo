namespace SoftPlus.Todo.Interfaces.DTOs.Task;

public sealed record UpdateTaskDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    int? CategoryId);