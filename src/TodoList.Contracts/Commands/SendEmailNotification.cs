namespace TodoList.Contracts.Commands;

public record SendEmailNotification(string To, string Subject, string Body);
