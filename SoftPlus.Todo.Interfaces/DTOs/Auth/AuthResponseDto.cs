namespace SoftPlus.Todo.Interfaces.DTOs.Auth;

public sealed record AuthResponseDto(string Token, string Email, int UserId);