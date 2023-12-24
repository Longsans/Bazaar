namespace Bazaar.FbbInventory.Application.Utilities;

public static class ResultExtensions
{
    public static string? GetJoinedErrorMessage<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return null;
        }

        return result.Status switch
        {
            ResultStatus.Invalid => string.Join("; ", result.ValidationErrors.Select(x => x.ErrorMessage)),
            _ => string.Join("; ", result.Errors)
        };
    }
}
