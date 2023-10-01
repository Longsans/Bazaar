namespace ContractingTests;

public static class TestDbSeeding
{
    public static void Reseed(this ContractingDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var uncontractedPartner = new Partner
        {
            ExternalId = "PNER-1",
            FirstName = "Test1",
            LastName = "Test1",
            Email = "Test@testmail.com",
            PhoneNumber = "0123456789",
            DateOfBirth = new DateTime(1989, 11, 10),
            Gender = Gender.Male,
        };

        var contractedPartner = new Partner
        {
            ExternalId = "PNER-2",
            FirstName = "Test2",
            LastName = "Test2",
            Email = "Test2@testmail.com",
            PhoneNumber = "9876543210",
            DateOfBirth = new DateTime(1990, 11, 10),
            Gender = Gender.Female,
        };

        var plan = new SellingPlan
        {
            Name = "Test plan",
            PerSaleFee = 10,
            MonthlyFee = 100,
            RegularPerSaleFeePercent = 0.05f,
        };

        var contract = new Contract
        {
            Partner = contractedPartner,
            SellingPlan = plan,
            StartDate = DateTime.Now.Date - TimeSpan.FromDays(7),
        };

        context.Partners.AddRange(uncontractedPartner, contractedPartner);
        context.SellingPlans.Add(plan);
        context.SaveChanges();

        context.Contracts.Add(contract);
        context.SaveChanges();
    }
}
