namespace TodoList.Api.Application.DTOs.Auth.Inputs;

public record ResetPasswordDto(string Token, string Password, string ConfirmPassword);
