using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.Common.Services;

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
