namespace Bazaar.Contracting.Core.Model;

public class Partner
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public List<Contract> Contracts { get; set; }

    public bool IsUnderContract
        => CurrentContract != null
        && (CurrentContract.EndDate == null
            || CurrentContract.EndDate > DateTime.Now.Date);

    public Contract? CurrentContract
        => Contracts.SingleOrDefault(c =>
            c.StartDate == Contracts.Max(c2 => c2.StartDate)
            && (c.EndDate is null || c.EndDate >= DateTime.Now.Date));
}

public enum Gender
{
    Male,
    Female,
    Other,
}