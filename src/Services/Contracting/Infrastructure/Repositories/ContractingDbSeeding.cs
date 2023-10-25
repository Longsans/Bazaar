namespace Bazaar.Contracting.Infrastructure.Repositories;

public static class ContractingDbSeeding
{
    private const string PARTNERS_SECTION = "clients";
    private const string SELLING_PLANS_SECTION = "sellingPlans";

    public static async Task Seed(this ContractingDbContext context, IServiceProvider sp)
    {
        context.Database.EnsureDeleted();
        await context.Database.MigrateAsync();

        if (context.Clients.Any())
        {
            return;
        }

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.Clients.AddRange(adapter.ReadToObjects<Client>(PARTNERS_SECTION));
        context.SellingPlans.AddRange(adapter.ReadToObjects<SellingPlan>(SELLING_PLANS_SECTION));
        await context.SaveChangesAsync();

        //var client = context.Clients.First();
        //var sellingPlan = context.SellingPlans.First();
        //var contract = new Contract
        //{
        //    Client = client,
        //    SellingPlan = sellingPlan,
        //    StartDate = DateTime.Now,
        //};
        //context.Contracts.Add(contract);
        //await context.SaveChangesAsync();
    }
}
