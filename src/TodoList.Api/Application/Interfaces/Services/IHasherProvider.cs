namespace TodoList.Api.Application.Interfaces.Services;

public interface IHasherProvider
{
    string HashText(string text);
    bool Verify(string text, string hashedText);
}
