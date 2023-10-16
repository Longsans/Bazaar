namespace Bazaar.Contracting.Domain.Exceptions;

public class ExtendIndefiniteContractException
    : Exception
{
    public ExtendIndefiniteContractException()
    : base("Cannot extend an indefinite contract.") { }
}
