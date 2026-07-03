namespace TodoList.Api.Features.Auth.Core.DTOs.Outputs;

public record UserResultDto(int Id, string Fullname, string Email, bool IsEmailVerified);
