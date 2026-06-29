using SoftPlus.Todo.Interfaces.DTOs.SubSteps;

namespace SoftPlus.Todo.Interfaces.DTOs.Task;

public sealed record TaskDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    int? CategoryId,
    string? CategoryName,
    IReadOnlyCollection<SubStepDto> SubSteps);