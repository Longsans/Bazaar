using Newtonsoft.Json;

namespace Bazaar.Contracting.Domain.Entities;

public class Client
{
    public int Id { get; private set; }
    public string ExternalId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }

    public int SellingPlanId { get; private set; }
    public SellingPlan SellingPlan { get; private set; }

    private readonly List<Contract> _contracts;
    public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

    public Contract CurrentContract
        => Contracts.Single(c =>
            c.StartDate == Contracts.Max(c2 => c2.StartDate) && c.EndDate == null);

    // Create constructor
    [JsonConstructor]
    public Client(
        string firstName,
        string lastName,
        string emailAddress,
        string phoneNumber,
        DateTime dateOfBirth,
        Gender gender,
        int sellingPlanId)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;

        DateOfBirth = DateTime.Now.Year - dateOfBirth.Year >= ClientCompliance.MinimumAge
            ? dateOfBirth.Date : throw new ClientUnderMinimumAgeException(ClientCompliance.MinimumAge);

        Gender = gender;
        SellingPlanId = sellingPlanId;

        var contract = new Contract(default, sellingPlanId);
        _contracts = new() { contract };
    }

    // EF Core read constructor
    public Client(
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

        DateOfBirth = DateTime.Now.Year - dateOfBirth.Year >= ClientCompliance.MinimumAge
            ? dateOfBirth.Date : throw new ClientUnderMinimumAgeException(ClientCompliance.MinimumAge);

        Gender = gender;
        _contracts = new();
    }

    public void ChangeSellingPlan(SellingPlan plan)
    {
        SellingPlan = plan;
        SellingPlanId = plan.Id;
        CurrentContract.End();
        var newContract = new Contract(Id, plan.Id);
        _contracts.Add(newContract);
    }

    public void ChangePersonalInfo(
        string firstName, string lastName, string phoneNumber,
        DateTime dateOfBirth, Gender gender)
    {
        DateOfBirth = DateTime.Now.Year - dateOfBirth.Year >= ClientCompliance.MinimumAge
            ? dateOfBirth.Date : throw new ClientUnderMinimumAgeException(ClientCompliance.MinimumAge);

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Gender = gender;
    }

    public void ChangeEmailAddress(string newEmailAddress)
    {
        if (string.IsNullOrWhiteSpace(newEmailAddress))
            throw new ArgumentNullException(nameof(newEmailAddress), "Email address cannot be empty.");

        EmailAddress = newEmailAddress;
    }
}