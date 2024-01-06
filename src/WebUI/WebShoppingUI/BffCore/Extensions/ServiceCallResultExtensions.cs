namespace WebShoppingUI.Extensions;

public static class ServiceCallResultExtensions
{
    public static ActionResult ToActionResult(this ServiceCallResult result, ActionResult? successResult = null)
    {
        if (result.IsSuccess)
        {
            return successResult ?? new OkResult();
        }
        return result.ErrorType switch
        {
            ServiceCallError.Unauthorized => new UnauthorizedResult(),
            ServiceCallError.NotFound => new NotFoundObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.Conflict => new ConflictObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.BadRequest => new BadRequestObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.InternalError => new ObjectResult(new { error = result.ErrorDetail }) { StatusCode = 500 },
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }

    public static ActionResult<T> ToActionResult<T>(this ServiceCallResult<T> result)
        where T : class
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }
        return result.ErrorType switch
        {
            ServiceCallError.Unauthorized => new UnauthorizedResult(),
            ServiceCallError.NotFound => new NotFoundObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.Conflict => new ConflictObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.BadRequest => new BadRequestObjectResult(new { error = result.ErrorDetail }),
            ServiceCallError.InternalError => new ObjectResult(new { error = result.ErrorDetail }) { StatusCode = 500 },
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }
}
