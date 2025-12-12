using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Common;

namespace ProductOrderApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult OkResponse<T>(T data, string? message = null)
            => Ok(ApiResponse<T>.Ok(data, message));

        protected IActionResult FailResponse<T>(string message, object? errors = null)
            => BadRequest(ApiResponse<T>.Fail(message, errors));
    }
}