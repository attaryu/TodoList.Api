using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Auth.Core.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    [MinLength(5)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
}
