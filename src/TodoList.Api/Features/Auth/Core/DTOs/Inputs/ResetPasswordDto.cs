namespace TodoList.Api.Features.Auth.Core.DTOs.Inputs;

public record ResetPasswordDto(string Token, string Password, string ConfirmPassword);
