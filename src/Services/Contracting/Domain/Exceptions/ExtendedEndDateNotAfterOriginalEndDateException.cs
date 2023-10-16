namespace Bazaar.Contracting.Domain.Exceptions;

public class ExtendedEndDateNotAfterOriginalEndDateException
    : Exception
{
    public ExtendedEndDateNotAfterOriginalEndDateException()
        : base("Extended end date must be after original end date.")
    { }
}
