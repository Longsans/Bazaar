namespace Bazaar.Contracting.Domain.Exceptions;

public class EndDateBeforeCurrentDateException
    : Exception
{
    public EndDateBeforeCurrentDateException()
        : base("Contract end date cannot be before current date.")
    { }
}
