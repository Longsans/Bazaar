namespace Bazaar.Contracting.Infrastructure.Repositories;

public static class ContractingDbSeeding
{
    private const string PARTNERS_SECTION = "partners";
    private const string SELLING_PLANS_SECTION = "sellingPlans";

    public static async Task Seed(this ContractingDbContext context, IServiceProvider sp)
    {
        context.Database.EnsureDeleted();
        await context.Database.MigrateAsync();

        if (context.Partners.Any())
        {
            return;
        }

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.Partners.AddRange(adapter.ReadToObjects<Partner>(PARTNERS_SECTION));
        context.SellingPlans.AddRange(adapter.ReadToObjects<SellingPlan>(SELLING_PLANS_SECTION));
        await context.SaveChangesAsync();

        //var partner = context.Partners.First();
        //var sellingPlan = context.SellingPlans.First();
        //var contract = new Contract
        //{
        //    Partner = partner,
        //    SellingPlan = sellingPlan,
        //    StartDate = DateTime.Now,
        //};
        //context.Contracts.Add(contract);
        //await context.SaveChangesAsync();
    }
}
