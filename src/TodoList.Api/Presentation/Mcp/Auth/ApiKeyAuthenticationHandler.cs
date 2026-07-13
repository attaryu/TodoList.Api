using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.Presentation.Mcp.Auth;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyService apiKeyService
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IApiKeyService _apiKeyService = apiKeyService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.NoResult();
        }

        string rawKey;
        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            rawKey = authorizationHeader["Bearer ".Length..].Trim();
        }
        else
        {
            rawKey = authorizationHeader.Trim();
        }

        var verifiedKey = await _apiKeyService.VerifyApiKeyAsync(rawKey);
        if (verifiedKey == null)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, verifiedKey.UserId.ToString()),
            new Claim("ApiKeyId", verifiedKey.ApiKeyId.ToString()),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";

        var response = Sindika.AspNet.Response.ResponseHelper.Error<object, object>(
            null,
            "ERR-AUTH-401",
            "API Key is missing, invalid, or expired."
        );

        var json = System.Text.Json.JsonSerializer.Serialize(response);
        await Response.WriteAsync(json);
    }
}
