namespace TodoList.Api.Shared.Contracts;

public record SendEmailNotification(string To, string Subject, string Body);
