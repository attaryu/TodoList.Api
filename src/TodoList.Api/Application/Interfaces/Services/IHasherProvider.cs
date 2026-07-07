namespace TodoList.Api.Features.Auth.Core.Providers;

public interface IHasherProvider
{
    string HashText(string text);
    bool Verify(string text, string hashedText);
}
