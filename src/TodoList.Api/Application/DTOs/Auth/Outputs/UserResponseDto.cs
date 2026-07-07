namespace TodoList.Api.Application.DTOs.Auth.Outputs;

public record UserResultDto(int Id, string Fullname, string Email, bool IsEmailVerified);
