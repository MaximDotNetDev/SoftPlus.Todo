namespace SoftPlus.Todo.Interfaces.DTOs.SubSteps;

public sealed record SubStepDto(int Id, string Title, bool IsCompleted, int TaskId);