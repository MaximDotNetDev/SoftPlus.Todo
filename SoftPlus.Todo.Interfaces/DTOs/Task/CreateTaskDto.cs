namespace SoftPlus.Todo.Interfaces.DTOs.Task;

public sealed record CreateTaskDto(
    string Title,
    string? Description,
    DateTime? DueDate,
    int? CategoryId);