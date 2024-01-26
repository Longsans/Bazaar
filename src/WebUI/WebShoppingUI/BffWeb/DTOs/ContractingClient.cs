namespace WebShoppingUI.DTOs;

public class ContractingClient
{
    public int Id { get; }
    public string ExternalId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public DateTime DateOfBirth { get; }
    public Gender Gender { get; }
    public SellingPlan SellingPlan { get; }
    public List<Contract> Contracts { get; }
    public Contract CurrentContract { get; }

    public ContractingClient(
        int id, string externalId, string firstName, string lastName,
        string email, string phoneNumber, DateTime dateOfBirth, Gender gender,
        SellingPlan sellingPlan, List<Contract> contracts, Contract currentContract)
    {
        Id = id;
        ExternalId = externalId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        SellingPlan = sellingPlan;
        Contracts = contracts;
        CurrentContract = currentContract;
    }
}
