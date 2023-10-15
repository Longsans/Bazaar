using Newtonsoft.Json;

namespace Bazaar.Contracting.Domain.Entities;

public class Partner
{
    public int Id { get; private set; }
    public string ExternalId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }

    private readonly List<Contract> _contracts;
    public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

    public Contract? CurrentContract
        => Contracts.SingleOrDefault(c =>
            c.StartDate == Contracts.Max(c2 => c2.StartDate)
            && (c.EndDate is null || c.EndDate >= DateTime.Now.Date));

    public bool IsUnderContract => CurrentContract != null
            && (CurrentContract.EndDate == null || CurrentContract.EndDate > DateTime.Now.Date);

    [JsonConstructor]
    public Partner(
        string firstName,
        string lastName,
        string emailAddress,
        string phoneNumber,
        DateTime dateOfBirth,
        Gender gender)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;

        DateOfBirth = DateTime.Now.Year - dateOfBirth.Year >= PartnerCompliance.MinimumAge
            ? dateOfBirth.Date : throw new PartnerUnderMinimumAgeException(PartnerCompliance.MinimumAge);

        Gender = gender;
        _contracts = new();
    }

    public Partner(
        int id,
        string externalId,
        string firstName,
        string lastName,
        string emailAddress,
        string phoneNumber,
        DateTime dateOfBirth,
        Gender gender) : this(firstName, lastName, emailAddress, phoneNumber, dateOfBirth, gender)
    {
        Id = id;
        ExternalId = externalId;
    }

    public void SignContract(Contract contract)
    {
        if (IsUnderContract)
            throw new PartnerAlreadyUnderContractException(ExternalId);

        _contracts.Add(contract);
    }

    public void ChangeEmailAddress(string newEmailAddress)
    {
        if (string.IsNullOrWhiteSpace(newEmailAddress))
            throw new ArgumentNullException(nameof(newEmailAddress));

        EmailAddress = newEmailAddress;
    }
}