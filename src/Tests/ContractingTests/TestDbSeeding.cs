namespace ContractingTests;

public static class TestDbSeeding
{
    //public static void Reseed(this ContractingDbContext context)
    //{
    //    context.Database.EnsureDeleted();
    //    context.Database.EnsureCreated();

    //    var client1NoContract = new Client
    //    {
    //        ExternalId = "CLNT-1",
    //        FirstName = "Test1",
    //        LastName = "Test1",
    //        Email = "Test@testmail.com",
    //        PhoneNumber = "0123456789",
    //        DateOfBirth = new DateTime(1989, 11, 10),
    //        Gender = Gender.Male,
    //    };

    //    var client2InContracted = new Client
    //    {
    //        ExternalId = "CLNT-2",
    //        FirstName = "Test2",
    //        LastName = "Test2",
    //        Email = "Test2@testmail.com",
    //        PhoneNumber = "9876543210",
    //        DateOfBirth = new DateTime(1990, 11, 10),
    //        Gender = Gender.Female,
    //    };

    //    var client3FpContracted = new Client
    //    {
    //        ExternalId = "CLNT-3",
    //        FirstName = "Test3",
    //        LastName = "Test3",
    //        Email = "Test3@testmail.com",
    //        PhoneNumber = "0123123123",
    //        DateOfBirth = new DateTime(1979, 11, 10),
    //        Gender = Gender.Female,
    //    };

    //    var client4FpContracted = new Client
    //    {
    //        ExternalId = "CLNT-4",
    //        FirstName = "Test4",
    //        LastName = "Test4",
    //        Email = "Test4@testmail.com",
    //        PhoneNumber = "0123123123",
    //        DateOfBirth = new DateTime(1969, 11, 10),
    //        Gender = Gender.Male,
    //    };

    //    var plan = new SellingPlan
    //    {
    //        Name = "Test plan",
    //        PerSaleFee = 10,
    //        MonthlyFee = 100,
    //        RegularPerSaleFeePercent = 0.05f,
    //    };

    //    var contract1p2In = new Contract
    //    {
    //        Client = client2InContracted,
    //        SellingPlan = plan,
    //        StartDate = DateTime.Now.Date - TimeSpan.FromDays(7),
    //    };

    //    var contract2p3Fp = new Contract
    //    {
    //        Client = client3FpContracted,
    //        SellingPlan = plan,
    //        StartDate = DateTime.Now.Date - TimeSpan.FromDays(14),
    //        EndDate = DateTime.Now.Date + TimeSpan.FromDays(14)
    //    };

    //    var contract3p4FpEndsToday = new Contract
    //    {
    //        Client = client4FpContracted,
    //        SellingPlan = plan,
    //        StartDate = DateTime.Now.Date - TimeSpan.FromDays(14),
    //        EndDate = DateTime.Now.Date
    //    };

    //    context.Clients.AddRange(
    //        client1NoContract,
    //        client2InContracted,
    //        client3FpContracted,
    //        client4FpContracted);
    //    context.SellingPlans.Add(plan);
    //    context.SaveChanges();

    //    context.Contracts.AddRange(
    //        contract1p2In,
    //        contract2p3Fp,
    //        contract3p4FpEndsToday);
    //    context.SaveChanges();
    //}
}
