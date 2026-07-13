using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Api.Presentation.Http.Controllers.Base;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }
        return userId;
    }
}
