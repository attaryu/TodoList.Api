using System.Net;
using System.Text.Json;
using Sindika.AspNet.Exceptions.BadRequest;
using Sindika.AspNet.Exceptions.Forbidden;
using Sindika.AspNet.Exceptions.NotFound;
using Sindika.AspNet.Exceptions.Unauthorized;

namespace TodoList.Api.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var code = "ERR-SRV-500";
        var message = "An internal server error occurred. Please try again later.";

        if (exception is UnauthorizedException unauthorizedException)
        {
            code = unauthorizedException.GetCode();
            message = unauthorizedException.GetUserMessage();
            statusCode = (int)unauthorizedException.StatusCode;
        }
        else if (exception is ForbiddenException forbiddenException)
        {
            code = forbiddenException.GetCode();
            message = forbiddenException.GetUserMessage();
            statusCode = (int)forbiddenException.StatusCode;
        }
        else if (exception is BadRequestException badRequestException)
        {
            code = badRequestException.GetCode();
            message = badRequestException.GetUserMessage();
            statusCode = (int)badRequestException.StatusCode;
        }
        else if (exception is NotFoundException notFoundException)
        {
            code = notFoundException.GetCode();
            message = notFoundException.GetUserMessage();
            statusCode = (int)notFoundException.StatusCode;
        }
        else if (exception is UnauthorizedAccessException ex)
        {
            code = "ERR-AUTH-401";
            message = ex.Message;
            statusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception is ArgumentException ex2)
        {
            code = "ERR-ARG-400";
            message = ex2.Message;
            statusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is InvalidOperationException ex3)
        {
            code = "ERR-OPR-400";
            message = ex3.Message;
            statusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            _logger.LogError(exception, exception.Message);
        }

        var response = new
        {
            success = false,
            code,
            message,
            error = (object?)null,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
