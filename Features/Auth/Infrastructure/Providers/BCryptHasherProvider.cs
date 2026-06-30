using TodoList.Api.Features.Auth.Core.Providers;

namespace TodoList.Api.Features.Auth.Infrastructure.Providers;

public class BCryptHasherProvider : IHasherProvider
{
    public string HashText(string text)
    {
        return BCrypt.Net.BCrypt.HashPassword(text);
    }

    public bool Verify(string text, string hashedText)
    {
        return BCrypt.Net.BCrypt.Verify(text, hashedText);
    }
}
