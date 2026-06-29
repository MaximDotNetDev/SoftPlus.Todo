using System.ComponentModel.DataAnnotations;

namespace SoftPlus.Todo.Interfaces.DTOs.Auth;

public sealed record LoginDto(
    [Required(ErrorMessage = "Email є обов'язковим.")]
    [MaxLength(100, ErrorMessage = "Email не може перевищувати 100 символів.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Некоректний формат Email (допускається лише латиниця, цифри та стандартні домени).")]
    string Email,

    [Required(ErrorMessage = "Пароль є обов'язковим.")]
    [MaxLength(64, ErrorMessage = "Пароль не може перевищувати 64 символи.")]
    string Password
);