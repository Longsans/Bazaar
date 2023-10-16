namespace Bazaar.Contracting.Domain.Exceptions;

public class ContractEndedException : Exception
{
    public ContractEndedException()
        : base("Contract has already ended.") { }
}
