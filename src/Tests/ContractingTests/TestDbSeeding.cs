namespace ContractingTests;

public static class TestDbSeeding
{
    public static void Reseed(this ContractingDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var partner1NoContract = new Partner
        {
            ExternalId = "PNER-1",
            FirstName = "Test1",
            LastName = "Test1",
            Email = "Test@testmail.com",
            PhoneNumber = "0123456789",
            DateOfBirth = new DateTime(1989, 11, 10),
            Gender = Gender.Male,
        };

        var partner2InContracted = new Partner
        {
            ExternalId = "PNER-2",
            FirstName = "Test2",
            LastName = "Test2",
            Email = "Test2@testmail.com",
            PhoneNumber = "9876543210",
            DateOfBirth = new DateTime(1990, 11, 10),
            Gender = Gender.Female,
        };

        var partner3FpContracted = new Partner
        {
            ExternalId = "PNER-3",
            FirstName = "Test3",
            LastName = "Test3",
            Email = "Test3@testmail.com",
            PhoneNumber = "0123123123",
            DateOfBirth = new DateTime(1979, 11, 10),
            Gender = Gender.Female,
        };

        var partner4FpContracted = new Partner
        {
            ExternalId = "PNER-4",
            FirstName = "Test4",
            LastName = "Test4",
            Email = "Test4@testmail.com",
            PhoneNumber = "0123123123",
            DateOfBirth = new DateTime(1969, 11, 10),
            Gender = Gender.Male,
        };

        var plan = new SellingPlan
        {
            Name = "Test plan",
            PerSaleFee = 10,
            MonthlyFee = 100,
            RegularPerSaleFeePercent = 0.05f,
        };

        var contract1p2In = new Contract
        {
            Partner = partner2InContracted,
            SellingPlan = plan,
            StartDate = DateTime.Now.Date - TimeSpan.FromDays(7),
        };

        var contract2p3Fp = new Contract
        {
            Partner = partner3FpContracted,
            SellingPlan = plan,
            StartDate = DateTime.Now.Date - TimeSpan.FromDays(14),
            EndDate = DateTime.Now.Date + TimeSpan.FromDays(14)
        };

        var contract3p4FpEndsToday = new Contract
        {
            Partner = partner4FpContracted,
            SellingPlan = plan,
            StartDate = DateTime.Now.Date - TimeSpan.FromDays(14),
            EndDate = DateTime.Now.Date
        };

        context.Partners.AddRange(
            partner1NoContract,
            partner2InContracted,
            partner3FpContracted,
            partner4FpContracted);
        context.SellingPlans.Add(plan);
        context.SaveChanges();

        context.Contracts.AddRange(
            contract1p2In,
            contract2p3Fp,
            contract3p4FpEndsToday);
        context.SaveChanges();
    }
}
