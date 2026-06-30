using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Auth.Core.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}
