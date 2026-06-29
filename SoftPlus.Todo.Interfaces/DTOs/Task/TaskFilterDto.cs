namespace SoftPlus.Todo.Interfaces.DTOs.Task;

public sealed record TaskFilterDto(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    int? CategoryId = null);