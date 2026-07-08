using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Api.API.Controllers.Base;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }
        return userId;
    }
}
