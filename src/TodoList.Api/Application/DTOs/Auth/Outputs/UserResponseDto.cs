namespace TodoList.Api.Application.DTOs.Auth.Outputs;

public record UserResultDto(Guid Id, string Fullname, string Email, bool IsEmailVerified);
