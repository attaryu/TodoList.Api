using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.Presentation.Mcp.Auth;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IHasherProvider _hasherProvider;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepository,
        IHasherProvider hasherProvider
    )
        : base(options, logger, encoder)
    {
        _apiKeyRepository = apiKeyRepository;
        _hasherProvider = hasherProvider;
    }

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

        if (string.IsNullOrEmpty(rawKey) || rawKey.Length < 12 || !rawKey.StartsWith("tlk_"))
        {
            return AuthenticateResult.Fail("Invalid API Key format.");
        }

        var prefix = rawKey[..12];
        var apiKeys = await _apiKeyRepository.GetActiveKeysByPrefixAsync(prefix);

        foreach (var apiKey in apiKeys)
        {
            if (_hasherProvider.Verify(rawKey, apiKey.KeyHash))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, apiKey.UserId.ToString()),
                    new Claim("ApiKeyId", apiKey.Id.ToString()),
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
        }

        return AuthenticateResult.Fail("Invalid API Key.");
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
